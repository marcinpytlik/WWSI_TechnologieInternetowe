using CourseApi.Data;
using CourseApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("CourseDb")
    ?? throw new InvalidOperationException("Connection string 'CourseDb' was not found.");

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSwagger();
app.UseSwaggerUI();

await InitializeDatabaseAsync(app);

app.MapGet("/api/topics", async (
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var topics = await db.Topics
        .AsNoTracking()
        .OrderBy(x => x.Id)
        .ToArrayAsync(cancellationToken);

    return Results.Ok(topics);
});

app.MapGet("/api/request-info", (HttpContext context) =>
{
    return Results.Ok(new
    {
        scheme = context.Request.Scheme,
        host = context.Request.Host.Value,
        remoteIp = context.Connection.RemoteIpAddress?.ToString()
    });
});

app.MapHealthChecks("/health");

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();

    await db.Database.EnsureCreatedAsync();

    if (!await db.Topics.AnyAsync())
    {
        db.Topics.AddRange(
            new CourseTopic { Name = "Nginx" },
            new CourseTopic { Name = "HTTPS" },
            new CourseTopic { Name = "Security headers" },
            new CourseTopic { Name = "Compression" },
            new CourseTopic { Name = "Caching" });

        await db.SaveChangesAsync();
    }
}
