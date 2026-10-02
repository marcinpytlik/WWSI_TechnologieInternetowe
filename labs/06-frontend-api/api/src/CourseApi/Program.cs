using CourseApi.Contracts;
using CourseApi.Data;
using CourseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("CourseDb")
    ?? throw new InvalidOperationException("Connection string 'CourseDb' was not found.");

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");

await InitializeDatabaseAsync(app);

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/topics", async (
    [FromQuery] string? search,
    [FromQuery] string? sort,
    [FromQuery] int page,
    [FromQuery] int pageSize,
    CourseDbContext db,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    page = page == 0 ? 1 : page;
    pageSize = pageSize == 0 ? 5 : pageSize;

    if (page < 1)
        return ValidationProblem(httpContext, "Page must be greater than or equal to 1.");

    if (pageSize < 1 || pageSize > 100)
        return ValidationProblem(httpContext, "PageSize must be between 1 and 100.");

    IQueryable<CourseTopic> query = db.Topics.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(search))
    {
        var term = search.Trim();
        query = query.Where(x => x.Name.Contains(term));
    }

    query = sort?.Trim().ToLowerInvariant() switch
    {
        "name" => query.OrderBy(x => x.Name),
        "-name" => query.OrderByDescending(x => x.Name),
        "id" => query.OrderBy(x => x.Id),
        "-id" => query.OrderByDescending(x => x.Id),
        _ => query.OrderBy(x => x.Id)
    };

    var totalItems = await query.CountAsync(cancellationToken);
    var totalPages = totalItems == 0
        ? 0
        : (int)Math.Ceiling(totalItems / (double)pageSize);

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new TopicResponse(x.Id, x.Name))
        .ToArrayAsync(cancellationToken);

    httpContext.Response.Headers["X-Total-Count"] = totalItems.ToString();

    return Results.Ok(new PagedResponse<TopicResponse>(
        items, page, pageSize, totalItems, totalPages));
});

app.MapPost("/api/topics", async (
    CreateTopicRequest request,
    CourseDbContext db,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    var validation = ValidateName(request.Name);
    if (validation is not null)
        return ValidationProblem(httpContext, validation);

    var normalized = request.Name.Trim();

    if (await db.Topics.AnyAsync(x => x.Name == normalized, cancellationToken))
    {
        return Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflict",
            detail: $"Topic '{normalized}' already exists.",
            instance: httpContext.Request.Path);
    }

    var topic = new CourseTopic { Name = normalized };

    db.Topics.Add(topic);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created(
        $"/api/topics/{topic.Id}",
        new TopicResponse(topic.Id, topic.Name));
});

app.MapPut("/api/topics/{id:int}", async (
    int id,
    UpdateTopicRequest request,
    CourseDbContext db,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    var validation = ValidateName(request.Name);
    if (validation is not null)
        return ValidationProblem(httpContext, validation);

    var topic = await db.Topics.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (topic is null)
        return NotFoundProblem(httpContext, $"Topic with id {id} was not found.");

    var normalized = request.Name.Trim();

    if (await db.Topics.AnyAsync(
        x => x.Id != id && x.Name == normalized,
        cancellationToken))
    {
        return Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflict",
            detail: $"Topic '{normalized}' already exists.",
            instance: httpContext.Request.Path);
    }

    topic.Name = normalized;
    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new TopicResponse(topic.Id, topic.Name));
});

app.MapDelete("/api/topics/{id:int}", async (
    int id,
    CourseDbContext db,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    var topic = await db.Topics.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (topic is null)
        return NotFoundProblem(httpContext, $"Topic with id {id} was not found.");

    db.Topics.Remove(topic);
    await db.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
});

app.MapHealthChecks("/health");

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

static string? ValidateName(string? name)
{
    if (string.IsNullOrWhiteSpace(name))
        return "Name is required.";

    var trimmed = name.Trim();

    if (trimmed.Length < 3)
        return "Name must contain at least 3 characters.";

    if (trimmed.Length > 100)
        return "Name cannot contain more than 100 characters.";

    return null;
}

static IResult ValidationProblem(HttpContext context, string detail) =>
    Results.Problem(
        statusCode: StatusCodes.Status400BadRequest,
        title: "Validation error",
        detail: detail,
        instance: context.Request.Path);

static IResult NotFoundProblem(HttpContext context, string detail) =>
    Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Resource not found",
        detail: detail,
        instance: context.Request.Path);
