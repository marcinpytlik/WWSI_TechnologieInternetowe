using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var cors = "_sqlDemoCors";

builder.Services.AddCors(o =>
{
    o.AddPolicy(cors, p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("AdventureWorks2022")!));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(cors);

app.MapGet("/", () => Results.Redirect("/swagger"));

// --------------------------------------------------------
// Option 1: Live telemetry
// --------------------------------------------------------

app.MapGet("/telemetry/snapshot", async ([FromServices] IDbConnectionFactory factory) =>
{
    await using var conn = await factory.OpenAsync();

    // 1) Aktywne sesje
    var activeSessions = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE is_user_process = 1;");

    // 2) Aktywne żądania
    var activeRequests = await ScalarAsync<int>(conn,
        @"SELECT COUNT(*) FROM sys.dm_exec_requests WHERE session_id > 50;");

    // 3) Top query wg CPU
    var topQueryText = await ScalarAsync<string?>(conn, @"
SELECT TOP (1)
    SUBSTRING(st.text,
              (qs.statement_start_offset/2)+1,
              CASE
                  WHEN qs.statement_end_offset = -1
                  THEN LEN(CONVERT(nvarchar(max), st.text)) - (qs.statement_start_offset/2)
                  ELSE (qs.statement_end_offset - qs.statement_start_offset)/2 + 1
              END) AS statement_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
ORDER BY qs.total_worker_time DESC;");

    // 4) Query Store
    int queryStoreQueries;
    try
    {
        queryStoreQueries = await ScalarAsync<int>(conn, @"
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = DB_NAME() AND is_query_store_on = 1)
    SELECT COUNT(*) FROM sys.query_store_query;
ELSE
    SELECT 0;
");
    }
    catch
    {
        queryStoreQueries = 0;
    }

    var snapshot = new
    {
        timestamp = DateTimeOffset.UtcNow,
        activeSessions,
        activeRequests,
        queryStoreQueries,
        topQueryText = topQueryText ?? "(brak danych / idle)"
    };

    return Results.Ok(snapshot);
})
.WithName("GetTelemetrySnapshot");

// --------------------------------------------------------
// Option 2: SQL Query Assistant (rule-based demo)
// --------------------------------------------------------

app.MapPost("/assistant/ask", async (
    [FromServices] IDbConnectionFactory factory,
    [FromBody] AskRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest("Question is required.");

    var sql = BuildSqlFromQuestion(request.Question);

    if (sql is null)
    {
        return Results.Ok(new AskResponse
        {
            Question = request.Question,
            Sql = "-- Nie znam odpowiedzi na to pytanie.\n" +
                  "-- Spróbuj np.: \"Pokaż klientów z Polski\" albo \"Top 10 produktów po cenie\".",
            Rows = new List<Dictionary<string, object?>>()
        });
    }

    await using var conn = await factory.OpenAsync();

    var rows = new List<Dictionary<string, object?>>();

    await using (var cmd = conn.CreateCommand())
    {
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
    }

    var response = new AskResponse
    {
        Question = request.Question,
        Sql = sql,
        Rows = rows
    };

    return Results.Ok(response);
})
.WithName("AskSqlAssistant");

app.Run();


// ===================== Helpers (top-level methods) =====================

static async Task<T> ScalarAsync<T>(SqlConnection conn, string sql)
{
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandType = CommandType.Text;

    var obj = await cmd.ExecuteScalarAsync();

    if (obj == null || obj is DBNull)
        return default!;

    if (obj is T t)
        return t;

    return (T)Convert.ChangeType(obj, typeof(T));
}

// Bardzo prosty „NLP” – reguły po słowach kluczowych
static string? BuildSqlFromQuestion(string question)
{
    var q = question.ToLowerInvariant();

    // Klienci z Polski
    if (q.Contains("klient") && (q.Contains("polsk") || q.Contains("pl")))
    {
        return @"
SELECT TOP (20)
    c.CustomerID,
    p.FirstName,
    p.LastName,
    a.City,
    sp.Name AS StateProvince,
    cr.Name AS CountryRegion
FROM Sales.Customer c
JOIN Person.Person p          ON c.PersonID = p.BusinessEntityID
JOIN Sales.SalesOrderHeader h ON h.CustomerID = c.CustomerID
JOIN Person.Address a         ON h.BillToAddressID = a.AddressID
JOIN Person.StateProvince sp  ON a.StateProvinceID = sp.StateProvinceID
JOIN Person.CountryRegion cr  ON sp.CountryRegionCode = cr.CountryRegionCode
WHERE cr.Name = N'Poland'
ORDER BY c.CustomerID;
";
    }

    // Top 10 produktów po cenie
    if (q.Contains("produkt") && (q.Contains("drogi") || q.Contains("najdro") || q.Contains("top 10")))
    {
        return @"
SELECT TOP (10)
    ProductID,
    Name,
    ProductNumber,
    ListPrice
FROM Production.Product
ORDER BY ListPrice DESC;
";
    }

    // Sprzedaż wg kraju
    if ((q.Contains("sprzeda") || q.Contains("sales")) && (q.Contains("kraj") || q.Contains("country")))
    {
        return @"
SELECT TOP (20)
    cr.Name       AS Country,
    SUM(soh.SubTotal) AS TotalSales
FROM Sales.SalesOrderHeader soh
JOIN Sales.SalesTerritory st ON soh.TerritoryID = st.TerritoryID
JOIN Person.CountryRegion cr ON st.CountryRegionCode = cr.CountryRegionCode
GROUP BY cr.Name
ORDER BY TotalSales DESC;
";
    }

    // Liczba zamówień klienta
    if ((q.Contains("ile") || q.Contains("liczba")) && q.Contains("zamówień") && q.Contains("klient"))
    {
        return @"
SELECT TOP (20)
    c.CustomerID,
    COUNT(*) AS OrderCount,
    SUM(soh.TotalDue) AS TotalDue
FROM Sales.SalesOrderHeader soh
JOIN Sales.Customer c ON soh.CustomerID = c.CustomerID
GROUP BY c.CustomerID
ORDER BY OrderCount DESC;
";
    }

    // Fallback – coś sensownego do demo
    if (q.Contains("klient") || q.Contains("customer"))
    {
        return @"
SELECT TOP (20)
    c.CustomerID,
    c.AccountNumber,
    c.PersonID
FROM Sales.Customer c
ORDER BY c.CustomerID;
";
    }

    if (q.Contains("produkt") || q.Contains("product"))
    {
        return @"
SELECT TOP (20)
    ProductID,
    Name,
    ProductNumber,
    ListPrice
FROM Production.Product
ORDER BY ProductID;
";
    }

    return null;
}


// ===================== Types (at the end) =====================

interface IDbConnectionFactory
{
    Task<SqlConnection> OpenAsync();
}

class SqlConnectionFactory : IDbConnectionFactory
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

public sealed class AskRequest
{
    public string Question { get; set; } = string.Empty;
}

public sealed class AskResponse
{
    public string Question { get; set; } = string.Empty;
    public string Sql { get; set; } = string.Empty;
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}
