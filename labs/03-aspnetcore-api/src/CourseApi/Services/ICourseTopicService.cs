using CourseApi.Models;

namespace CourseApi.Services;

public interface ICourseTopicService
{
    IReadOnlyCollection<CourseTopic> GetAll();
    CourseTopic? GetById(int id);
    CourseTopic Add(string name);
}
