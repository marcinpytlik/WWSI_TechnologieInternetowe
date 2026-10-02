using System.Security.Claims;
using System.Text;
using CourseApi.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<TokenService>();

var signingKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

var issuer = builder.Configuration["Jwt:Issuer"] ?? "CourseApi";
var audience = builder.Configuration["Jwt:Audience"] ?? "CourseFrontend";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

var topics = new List<Topic>
{
    new(1, "HTTP"),
    new(2, "ASP.NET Core"),
    new(3, "JWT")
};
var nextId = 4;

app.MapPost("/api/auth/login", (LoginRequest request, IUserService users, TokenService tokens) =>
{
    var user = users.Validate(request.Username, request.Password);

    if (user is null)
        return Results.Unauthorized();

    var token = tokens.Create(user);
    return Results.Ok(new LoginResponse(token.Token, token.ExpiresAtUtc, user.Username, user.Role));
});

app.MapGet("/api/profile", (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        username = user.Identity?.Name,
        role = user.FindFirstValue(ClaimTypes.Role)
    });
}).RequireAuthorization();

app.MapGet("/api/topics", () => Results.Ok(topics));

app.MapPost("/api/topics", (TopicRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest();

    var topic = new Topic(nextId++, request.Name.Trim());
    topics.Add(topic);
    return Results.Created($"/api/topics/{topic.Id}", topic);
}).RequireAuthorization();

app.MapPut("/api/topics/{id:int}", (int id, TopicRequest request) =>
{
    var index = topics.FindIndex(x => x.Id == id);
    if (index < 0) return Results.NotFound();

    topics[index] = new Topic(id, request.Name.Trim());
    return Results.Ok(topics[index]);
}).RequireAuthorization();

app.MapDelete("/api/topics/{id:int}", (int id) =>
{
    var topic = topics.FirstOrDefault(x => x.Id == id);
    if (topic is null) return Results.NotFound();

    topics.Remove(topic);
    return Results.NoContent();
}).RequireAuthorization("AdminOnly");

app.Run();

public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
public sealed record TopicRequest(string Name);
public sealed record Topic(int Id, string Name);
