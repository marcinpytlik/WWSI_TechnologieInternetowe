using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
{
    var cs = builder.Configuration.GetConnectionString("AdventureWorks2022")
             ?? throw new InvalidOperationException("Brak ConnectionStrings:Sql w appsettings.json");
    return new SqlConnectionFactory(cs);
});

var cors = "_sqlTelemetryProCors";

builder.Services.AddCors(o =>
{
    o.AddPolicy(cors, p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(cors);

app.MapGet("/", () => Results.Redirect("/swagger"));

// =====================================================================
//  TELEMETRY DASHBOARD – główny endpoint dla frontu
// =====================================================================

app.MapGet("/telemetry/dashboard", async ([FromServices] IDbConnectionFactory factory) =>
{
    await using var conn = await factory.OpenAsync();

    // 1) Aktywne sesje / żądania
    var activeSessions = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE is_user_process = 1;");

    var activeRequests = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_requests WHERE session_id > 50;");

    // 2) TOP waits (bez SLEEP, BROKER i innych „szumów”)
    var waits = await QueryAsync(conn, @"
WITH waits AS (
    SELECT TOP (10)
        wait_type,
        wait_time_ms,
        signal_wait_time_ms,
        wait_time_ms - signal_wait_time_ms AS resource_wait_ms
    FROM sys.dm_os_wait_stats
    WHERE wait_type NOT LIKE 'SLEEP%'
      AND wait_type NOT LIKE 'XE_TIMER%'
      AND wait_type NOT LIKE 'BROKER_%'
      AND wait_type NOT IN ('SOS_SCHEDULER_YIELD')
    ORDER BY wait_time_ms DESC
)
SELECT
    wait_type,
    wait_time_ms,
    signal_wait_time_ms,
    resource_wait_ms,
    CAST(100.0 * wait_time_ms / SUM(wait_time_ms) OVER() AS DECIMAL(5,2)) AS pct
FROM waits
ORDER BY wait_time_ms DESC;
");

    // 3) Top queries wg CPU
    var topCpuQueries = await QueryAsync(conn, @"
SELECT TOP (5)
    DB_NAME(st.dbid)          AS db_name,
    OBJECT_NAME(st.objectid, st.dbid) AS object_name,
    qs.execution_count,
    qs.total_worker_time/1000          AS cpu_ms,
    qs.total_elapsed_time/1000         AS duration_ms,
    SUBSTRING(st.text,
              (qs.statement_start_offset/2)+1,
              CASE
                  WHEN qs.statement_end_offset = -1
                  THEN LEN(CONVERT(nvarchar(max), st.text)) - (qs.statement_start_offset/2)
                  ELSE (qs.statement_end_offset-qs.statement_start_offset)/2+1
              END) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
ORDER BY qs.total_worker_time DESC;
");

    // 4) Tempdb usage (wersja demo – rozmiar plików)
    var tempdbUsage = await QuerySingleAsync(conn, @"
USE tempdb;
SELECT
    SUM(size)*8/1024.0 AS total_mb,
    SUM(CASE WHEN type_desc = 'ROWS' THEN size END)*8/1024.0 AS data_mb,
    SUM(CASE WHEN type_desc = 'LOG'  THEN size END)*8/1024.0 AS log_mb
FROM sys.database_files;
");

    // 5) Log stats (DM_IO_VIRTUAL_FILE_STATS – log bieżącej bazy)
    var logStats = await QuerySingleAsync(conn, @"
DECLARE @dbid int = DB_ID();
SELECT TOP (1)
    num_of_reads,
    num_of_writes,
    num_of_bytes_read,
    num_of_bytes_written
FROM sys.dm_io_virtual_file_stats(@dbid, 2); -- 2 = log file
");

    // 6) Query Store info (jeśli dostępny)
    int queryStoreOn = 0;
    int queryStoreQueries = 0;
    try
    {
        var row = await QuerySingleAsync(conn, @"
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = DB_NAME() AND is_query_store_on = 1)
    SELECT 1 AS qs_on, (SELECT COUNT(*) FROM sys.query_store_query) AS qs_queries;
ELSE
    SELECT 0 AS qs_on, 0 AS qs_queries;
");
        queryStoreOn = Convert.ToInt32(row["qs_on"] ?? 0);
        queryStoreQueries = Convert.ToInt32(row["qs_queries"] ?? 0);
    }
    catch
    {
        queryStoreOn = 0;
        queryStoreQueries = 0;
    }

    var payload = new
    {
        timestamp = DateTimeOffset.UtcNow,
        activeSessions,
        activeRequests,
        waits,
        topCpuQueries,
        tempdb = tempdbUsage,
        log = logStats,
        queryStore = new
        {
            isOn = queryStoreOn == 1,
            totalQueries = queryStoreQueries
        }
    };

    return Results.Ok(payload);
});

// =====================================================================
//  TELEMETRY PULSE – szybki endpoint do częstego odpytywania (2–3 s)
// =====================================================================

app.MapGet("/telemetry/pulse", async ([FromServices] IDbConnectionFactory factory) =>
{
    await using var conn = await factory.OpenAsync();

    var activeSessions = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE is_user_process = 1;");

    var activeRequests = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_requests WHERE session_id > 50;");

    // WaitType summary – tylko TOP 3
    var waits = await QueryAsync(conn, @"
WITH waits AS (
    SELECT TOP (3)
        wait_type,
        wait_time_ms
    FROM sys.dm_os_wait_stats
    WHERE wait_type NOT LIKE 'SLEEP%'
      AND wait_type NOT LIKE 'XE_TIMER%'
      AND wait_type NOT LIKE 'BROKER_%'
    ORDER BY wait_time_ms DESC
)
SELECT
    wait_type,
    wait_time_ms,
    CAST(100.0 * wait_time_ms / SUM(wait_time_ms) OVER() AS DECIMAL(5,2)) AS pct
FROM waits
ORDER BY wait_time_ms DESC;
");

    var pulse = new
    {
        timestamp = DateTimeOffset.UtcNow,
        activeSessions,
        activeRequests,
        waits
    };

    return Results.Ok(pulse);
});

// =====================================================================
//  ADVISOR – proste reguły SQLManiaka na bazie waitów + loga + tempdb
// =====================================================================

app.MapGet("/telemetry/advisor", async ([FromServices] IDbConnectionFactory factory) =>
{
    await using var conn = await factory.OpenAsync();

    var waits = await QueryAsync(conn, @"
WITH waits AS (
    SELECT TOP (10)
        wait_type,
        wait_time_ms
    FROM sys.dm_os_wait_stats
    WHERE wait_type NOT LIKE 'SLEEP%'
      AND wait_type NOT LIKE 'XE_TIMER%'
      AND wait_type NOT LIKE 'BROKER_%'
    ORDER BY wait_time_ms DESC
)
SELECT
    wait_type,
    wait_time_ms,
    CAST(100.0 * wait_time_ms / SUM(wait_time_ms) OVER() AS DECIMAL(5,2)) AS pct
FROM waits
ORDER BY wait_time_ms DESC;
");

    var tempdb = await QuerySingleAsync(conn, @"
USE tempdb;
SELECT
    SUM(size)*8/1024.0 AS total_mb
FROM sys.database_files;
");

    var logStats = await QuerySingleAsync(conn, @"
DECLARE @dbid int = DB_ID();
SELECT TOP (1)
    num_of_writes,
    num_of_bytes_written
FROM sys.dm_io_virtual_file_stats(@dbid, 2);
");

    var advice = BuildAdvice(waits, tempdb, logStats);

    return Results.Ok(new
    {
        timestamp = DateTimeOffset.UtcNow,
        waits,
        tempdb,
        log = logStats,
        advice.Severity,
        advice.Messages
    });
});

// =====================================================================
//  QUERY EXPLORER – demo: tylko SELECT, bez parametrowania
// =====================================================================

app.MapPost("/telemetry/query-exec", async (
    [FromServices] IDbConnectionFactory factory,
    [FromBody] QueryRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Sql))
        return Results.BadRequest("SQL is required.");

    var sql = req.Sql.Trim();
    if (!sql.StartsWith("select", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("Demo only allows SELECT statements.");

    await using var conn = await factory.OpenAsync();

    var rows = new List<Dictionary<string, object?>>();

    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 30;

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var dict = new Dictionary<string, object?>(reader.FieldCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
            }
            rows.Add(dict);
        }
    }

    return Results.Ok(new QueryResponse
    {
        Sql = sql,
        RowCount = rows.Count,
        Rows = rows
    });
});

app.Run();

// =====================================================================
//  HELPERY
// =====================================================================

static async Task<T> ScalarAsync<T>(SqlConnection conn, string sql)
{
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandType = CommandType.Text;
    var obj = await cmd.ExecuteScalarAsync();
    if (obj is null || obj is DBNull)
        return default!;
    if (obj is T t)
        return t;
    return (T)Convert.ChangeType(obj, typeof(T));
}

static async Task<Dictionary<string, object?>> QuerySingleAsync(SqlConnection conn, string sql)
{
    var list = await QueryAsync(conn, sql);
    return list.FirstOrDefault() ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}

static async Task<List<Dictionary<string, object?>>> QueryAsync(SqlConnection conn, string sql)
{
    var rows = new List<Dictionary<string, object?>>();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandType = CommandType.Text;

    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var dict = new Dictionary<string, object?>(reader.FieldCount, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < reader.FieldCount; i++)
        {
            dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
        }
        rows.Add(dict);
    }

    return rows;
}

// Prosty advisor oparty o waits + tempdb + log
static TelemetryAdvice BuildAdvice(
    List<Dictionary<string, object?>> waits,
    Dictionary<string, object?> tempdb,
    Dictionary<string, object?> log)
{
    var messages = new List<string>();
    var severity = "Info";

    Dictionary<string, decimal> waitsByType = new(StringComparer.OrdinalIgnoreCase);
    foreach (var w in waits)
    {
        var type = (w["wait_type"]?.ToString() ?? "");
        var pct = w["pct"] is decimal d ? d : Convert.ToDecimal(w["pct"] ?? 0);
        waitsByType[type] = pct;
    }

    bool HasWait(string namePart, decimal threshold)
    {
        return waitsByType
            .Where(kv => kv.Key.Contains(namePart, StringComparison.OrdinalIgnoreCase))
            .Any(kv => kv.Value >= threshold);
    }

    if (HasWait("PAGEIOLATCH", 20))
    {
        severity = "Warning";
        messages.Add("PAGEIOLATCH_* > 20% – presja IO na dane. Sprawdź storage, missing indexy, rozkład workloadu.");
    }

    if (HasWait("WRITELOG", 20))
    {
        severity = "Warning";
        messages.Add("WRITELOG > 20% – log jest wąskim gardłem. Sprawdź dysk pod log, rozmiar VLF, backupy logów, długie transakcje.");
    }

    if (HasWait("THREADPOOL", 1))
    {
        severity = "Critical";
        messages.Add("THREADPOOL – brakuje workerów. To poważny sygnał – wąskie gardło schedulerów. Sprawdź równoległość i bursty zapytań.");
    }

    if (HasWait("CXPACKET", 30) || HasWait("CXCONSUMER", 30))
    {
        if (severity != "Critical") severity = "Warning";
        messages.Add("CXPACKET/CXCONSUMER > 30% – potencjalne problemy z równoległością. Sprawdź MAXDOP, Cost Threshold for Parallelism.");
    }

    var tempdbMb = tempdb.TryGetValue("total_mb", out var tmbObj)
        ? Convert.ToDecimal(tmbObj ?? 0)
        : 0m;

    if (tempdbMb > 1024)
    {
        if (severity != "Critical") severity = "Warning";
        messages.Add($"Tempdb ~{tempdbMb:F0} MB – warto przejrzeć kto generuje duże sorty/hash/wersjonowanie.");
    }

    var logWrites = log.TryGetValue("num_of_writes", out var lwObj)
        ? Convert.ToInt64(lwObj ?? 0)
        : 0L;

    if (logWrites > 1_000_000)
    {
        if (severity != "Critical") severity = "Warning";
        messages.Add("Wysoka liczba zapisów do loga od startu instancji – intensywne OLTP lub batch. Zastanów się nad strategią batchowania/kompresji.");
    }

    if (messages.Count == 0)
        messages.Add("Brak oczywistych anomalii na podstawie waits/tempdb/log. Ale to nie znaczy, że wszystko idealne – to tylko prosty advisor demo :)");

    return new TelemetryAdvice
    {
        Severity = severity,
        Messages = messages
    };
}

// =====================================================================
//  TYPY
// =====================================================================

public interface IDbConnectionFactory
{
    Task<SqlConnection> OpenAsync();
}

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connString;
    public SqlConnectionFactory(string connString) => _connString = connString;

    public async Task<SqlConnection> OpenAsync()
    {
        var conn = new SqlConnection(_connString);
        await conn.OpenAsync();
        return conn;
    }
}

public sealed class QueryRequest
{
    public string Sql { get; set; } = string.Empty;
}

public sealed class QueryResponse
{
    public string Sql { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}

public sealed class TelemetryAdvice
{
    public string Severity { get; set; } = "Info";
    public List<string> Messages { get; set; } = new();
}
