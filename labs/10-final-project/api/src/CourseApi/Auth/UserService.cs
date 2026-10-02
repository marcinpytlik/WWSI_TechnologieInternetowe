namespace CourseApi.Auth;

public sealed class UserService(IConfiguration configuration) : IUserService
{
    public UserAccount? Validate(string username, string password)
    {
        var userPassword = configuration["DemoUsers:UserPassword"];
        var adminPassword = configuration["DemoUsers:AdminPassword"];

        if (string.Equals(username, "user", StringComparison.OrdinalIgnoreCase)
            && password == userPassword)
            return new UserAccount("user", password, "User");

        if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase)
            && password == adminPassword)
            return new UserAccount("admin", password, "Admin");

        return null;
    }
}
