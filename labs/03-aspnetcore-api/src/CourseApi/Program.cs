using CourseApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICourseTopicService, CourseTopicService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var started = DateTime.UtcNow;

    await next();

    var elapsed = DateTime.UtcNow - started;

    app.Logger.LogInformation(
        "Request {Method} {Path} -> {StatusCode} in {ElapsedMs} ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        elapsed.TotalMilliseconds);
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/topics", (
    ICourseTopicService service,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Returning all course topics");

    return Results.Ok(service.GetAll());
})
.WithName("GetTopics")
.WithOpenApi();

app.MapGet("/api/topics/{id:int}", (
    int id,
    ICourseTopicService service,
    ILogger<Program> logger) =>
{
    var topic = service.GetById(id);

    if (topic is null)
    {
        logger.LogWarning("Topic {TopicId} was not found", id);
        return Results.NotFound();
    }

    return Results.Ok(topic);
})
.WithName("GetTopicById")
.WithOpenApi();

app.MapPost("/api/topics", (
    CreateTopicRequest request,
    ICourseTopicService service,
    ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new
        {
            error = "Name is required."
        });
    }

    var topic = service.Add(request.Name);

    logger.LogInformation(
        "Created topic {TopicId}: {TopicName}",
        topic.Id,
        topic.Name);

    return Results.Created($"/api/topics/{topic.Id}", topic);
})
.WithName("CreateTopic")
.WithOpenApi();

app.MapGet("/api/info", (IConfiguration configuration) =>
{
    var courseName = configuration["Course:Name"]
                     ?? "Technologie Internetowe";

    return Results.Ok(new
    {
        name = courseName,
        runtime = Environment.Version.ToString(),
        environment = app.Environment.EnvironmentName
    });
})
.WithName("GetCourseInfo")
.WithOpenApi();

app.MapHealthChecks("/health");

app.Run();

public sealed record CreateTopicRequest(string Name);
