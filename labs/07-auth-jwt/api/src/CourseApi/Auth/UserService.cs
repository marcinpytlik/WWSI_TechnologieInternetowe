namespace CourseApi.Auth;

public sealed class UserService(IConfiguration configuration) : IUserService
{
    public UserAccount? Validate(string username, string password)
    {
        var studentPassword = configuration["DemoUsers:StudentPassword"];
        var adminPassword = configuration["DemoUsers:AdminPassword"];

        if (string.Equals(username, "student", StringComparison.OrdinalIgnoreCase)
            && password == studentPassword)
        {
            return new UserAccount("student", password, "Student");
        }

        if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase)
            && password == adminPassword)
        {
            return new UserAccount("admin", password, "Admin");
        }

        return null;
    }
}
