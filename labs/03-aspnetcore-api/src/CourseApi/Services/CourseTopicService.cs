using CourseApi.Models;

namespace CourseApi.Services;

public sealed class CourseTopicService : ICourseTopicService
{
    private readonly List<CourseTopic> _topics =
    [
        new(1, "HTTP"),
        new(2, "HTML, CSS i JavaScript"),
        new(3, "ASP.NET Core Web API")
    ];

    private int _nextId = 4;

    public IReadOnlyCollection<CourseTopic> GetAll() => _topics.AsReadOnly();

    public CourseTopic? GetById(int id) =>
        _topics.FirstOrDefault(x => x.Id == id);

    public CourseTopic Add(string name)
    {
        var topic = new CourseTopic(_nextId++, name.Trim());
        _topics.Add(topic);
        return topic;
    }
}
