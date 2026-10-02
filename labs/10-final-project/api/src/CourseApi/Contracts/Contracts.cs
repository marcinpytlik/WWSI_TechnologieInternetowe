namespace CourseApi.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
public sealed record CreateTopicRequest(string Name, string? Description);
public sealed record UpdateTopicRequest(string Name, string? Description);
public sealed record TopicResponse(int Id, string Name, string? Description, DateTime CreatedAt);
