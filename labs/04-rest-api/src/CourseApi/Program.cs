using CourseApi.Contracts;
using CourseApi.Models;
using CourseApi.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<ICourseTopicService, CourseTopicService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/topics", (
    [FromQuery] string? search,
    [FromQuery] string? sort,
    [FromQuery] int page,
    [FromQuery] int pageSize,
    ICourseTopicService service,
    HttpContext httpContext) =>
{
    page = page == 0 ? 1 : page;
    pageSize = pageSize == 0 ? 5 : pageSize;

    if (page < 1)
    {
        return ValidationProblem(
            httpContext,
            "Page must be greater than or equal to 1.");
    }

    if (pageSize < 1 || pageSize > 100)
    {
        return ValidationProblem(
            httpContext,
            "PageSize must be between 1 and 100.");
    }

    IEnumerable<CourseTopic> query = service.GetAll();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x =>
            x.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    query = sort?.Trim().ToLowerInvariant() switch
    {
        "name" => query.OrderBy(x => x.Name),
        "-name" => query.OrderByDescending(x => x.Name),
        "id" => query.OrderBy(x => x.Id),
        "-id" => query.OrderByDescending(x => x.Id),
        null or "" => query.OrderBy(x => x.Id),
        _ => query.OrderBy(x => x.Id)
    };

    var totalItems = query.Count();
    var totalPages = totalItems == 0
        ? 0
        : (int)Math.Ceiling(totalItems / (double)pageSize);

    var items = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(ToResponse)
        .ToArray();

    httpContext.Response.Headers["X-Total-Count"] = totalItems.ToString();

    return Results.Ok(
        new PagedResponse<TopicResponse>(
            items,
            page,
            pageSize,
            totalItems,
            totalPages));
})
.WithName("GetTopics")
.WithOpenApi();

app.MapGet("/api/topics/{id:int}", (
    int id,
    ICourseTopicService service,
    HttpContext httpContext) =>
{
    var topic = service.GetById(id);

    return topic is null
        ? NotFoundProblem(httpContext, $"Topic with id {id} was not found.")
        : Results.Ok(ToResponse(topic));
})
.WithName("GetTopicById")
.WithOpenApi();

app.MapPost("/api/topics", (
    CreateTopicRequest request,
    ICourseTopicService service,
    HttpContext httpContext) =>
{
    var validation = ValidateName(request.Name);

    if (validation is not null)
    {
        return ValidationProblem(httpContext, validation);
    }

    if (service.GetByName(request.Name) is not null)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflict",
            detail: $"Topic '{request.Name.Trim()}' already exists.",
            instance: httpContext.Request.Path);
    }

    var topic = service.Add(request.Name);

    return Results.Created(
        $"/api/topics/{topic.Id}",
        ToResponse(topic));
})
.WithName("CreateTopic")
.WithOpenApi();

app.MapPut("/api/topics/{id:int}", (
    int id,
    UpdateTopicRequest request,
    ICourseTopicService service,
    HttpContext httpContext) =>
{
    var validation = ValidateName(request.Name);

    if (validation is not null)
    {
        return ValidationProblem(httpContext, validation);
    }

    var duplicate = service.GetByName(request.Name);

    if (duplicate is not null && duplicate.Id != id)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflict",
            detail: $"Topic '{request.Name.Trim()}' already exists.",
            instance: httpContext.Request.Path);
    }

    var topic = service.Update(id, request.Name);

    return topic is null
        ? NotFoundProblem(httpContext, $"Topic with id {id} was not found.")
        : Results.Ok(ToResponse(topic));
})
.WithName("UpdateTopic")
.WithOpenApi();

app.MapDelete("/api/topics/{id:int}", (
    int id,
    ICourseTopicService service,
    HttpContext httpContext) =>
{
    return service.Delete(id)
        ? Results.NoContent()
        : NotFoundProblem(httpContext, $"Topic with id {id} was not found.");
})
.WithName("DeleteTopic")
.WithOpenApi();

app.Run();

static TopicResponse ToResponse(CourseTopic topic) =>
    new(topic.Id, topic.Name);

static string? ValidateName(string? name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return "Name is required.";
    }

    var trimmed = name.Trim();

    if (trimmed.Length < 3)
    {
        return "Name must contain at least 3 characters.";
    }

    if (trimmed.Length > 100)
    {
        return "Name cannot contain more than 100 characters.";
    }

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
