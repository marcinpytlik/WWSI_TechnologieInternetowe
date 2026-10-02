using CourseApi.Data;
using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("CourseDb")
    ?? throw new InvalidOperationException("Connection string 'CourseDb' was not found.");

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

await InitializeDatabaseAsync(app);

app.MapGet("/", () => Results.Redirect("/swagger"));

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

app.MapPost("/api/topics", async (
    TopicRequest request,
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var topic = new CourseTopic { Name = request.Name.Trim() };
    db.Topics.Add(topic);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/topics/{topic.Id}", topic);
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
            new CourseTopic { Name = "Docker" },
            new CourseTopic { Name = "Docker Compose" },
            new CourseTopic { Name = "Containers" });

        await db.SaveChangesAsync();
    }
}

public sealed record TopicRequest(string Name);
