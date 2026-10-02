namespace CourseApi.Auth;

public interface IUserService
{
    UserAccount? Validate(string username, string password);
}
