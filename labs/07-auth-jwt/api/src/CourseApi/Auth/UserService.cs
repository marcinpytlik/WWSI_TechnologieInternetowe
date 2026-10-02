namespace CourseApi.Auth;

public sealed class UserService : IUserService
{
    private static readonly UserAccount[] Users =
    [
        new("student", "Student123!", "Student"),
        new("admin", "Admin123!", "Admin")
    ];

    public UserAccount? Validate(string username, string password) =>
        Users.FirstOrDefault(x =>
            string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase)
            && x.Password == password);
}
