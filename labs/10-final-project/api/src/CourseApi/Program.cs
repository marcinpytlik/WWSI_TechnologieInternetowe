using System.Security.Claims;
using System.Text;
using CourseApi.Auth;
using CourseApi.Contracts;
using CourseApi.Data;
using CourseApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<TokenService>();

var connectionString = builder.Configuration.GetConnectionString("CourseDb")
    ?? throw new InvalidOperationException("Connection string 'CourseDb' was not found.");

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString));

var signingKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "CourseApi",
            ValidateAudience = true,
            ValidAudience = "CourseFrontend",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

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
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

await InitializeDatabaseAsync(app);

app.MapPost("/api/auth/login", (LoginRequest request, IUserService users, TokenService tokens) =>
{
    var user = users.Validate(request.Username, request.Password);

    if (user is null)
        return Results.Unauthorized();

    var token = tokens.Create(user);

    return Results.Ok(new LoginResponse(
        token.Token,
        token.ExpiresAtUtc,
        user.Username,
        user.Role));
});

app.MapGet("/api/profile", (ClaimsPrincipal user) =>
    Results.Ok(new
    {
        username = user.Identity?.Name,
        role = user.FindFirstValue(ClaimTypes.Role)
    }))
    .RequireAuthorization();

app.MapGet("/api/topics", async (
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var items = await db.Topics
        .AsNoTracking()
        .OrderBy(x => x.Id)
        .Select(x => new TopicResponse(
            x.Id,
            x.Name,
            x.Description,
            x.CreatedAt))
        .ToArrayAsync(cancellationToken);

    return Results.Ok(items);
});

app.MapGet("/api/topics/{id:int}", async (
    int id,
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var item = await db.Topics
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new TopicResponse(
            x.Id,
            x.Name,
            x.Description,
            x.CreatedAt))
        .SingleOrDefaultAsync(cancellationToken);

    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/topics", async (
    CreateTopicRequest request,
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest();

    var name = request.Name.Trim();

    if (await db.Topics.AnyAsync(x => x.Name == name, cancellationToken))
        return Results.Conflict();

    var topic = new CourseTopic
    {
        Name = name,
        Description = request.Description?.Trim()
    };

    db.Topics.Add(topic);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created(
        $"/api/topics/{topic.Id}",
        new TopicResponse(topic.Id, topic.Name, topic.Description, topic.CreatedAt));
})
.RequireAuthorization();

app.MapPut("/api/topics/{id:int}", async (
    int id,
    UpdateTopicRequest request,
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var topic = await db.Topics.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (topic is null)
        return Results.NotFound();

    topic.Name = request.Name.Trim();
    topic.Description = request.Description?.Trim();

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new TopicResponse(
        topic.Id,
        topic.Name,
        topic.Description,
        topic.CreatedAt));
})
.RequireAuthorization();

app.MapDelete("/api/topics/{id:int}", async (
    int id,
    CourseDbContext db,
    CancellationToken cancellationToken) =>
{
    var topic = await db.Topics.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    if (topic is null)
        return Results.NotFound();

    db.Topics.Remove(topic);
    await db.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
})
.RequireAuthorization("AdminOnly");

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
            new CourseTopic { Name = "HTTP", Description = "Podstawy protokołu HTTP" },
            new CourseTopic { Name = "ASP.NET Core", Description = "Web API" },
            new CourseTopic { Name = "Docker", Description = "Konteneryzacja" });

        await db.SaveChangesAsync();
    }
}
