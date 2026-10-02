using CourseApi.Models;

namespace CourseApi.Services;

public interface ICourseTopicService
{
    IReadOnlyCollection<CourseTopic> GetAll();
    CourseTopic? GetById(int id);
    CourseTopic? GetByName(string name);
    CourseTopic Add(string name);
    CourseTopic? Update(int id, string name);
    bool Delete(int id);
}
