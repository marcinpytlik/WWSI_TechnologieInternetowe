using CourseApi.Models;

namespace CourseApi.Services;

public sealed class CourseTopicService : ICourseTopicService
{
    private readonly List<CourseTopic> _topics =
    [
        new(1, "HTTP"),
        new(2, "HTML, CSS i JavaScript"),
        new(3, "ASP.NET Core Web API"),
        new(4, "REST API"),
        new(5, "Docker")
    ];

    private int _nextId = 6;

    public IReadOnlyCollection<CourseTopic> GetAll() => _topics.AsReadOnly();

    public CourseTopic? GetById(int id) =>
        _topics.FirstOrDefault(x => x.Id == id);

    public CourseTopic? GetByName(string name) =>
        _topics.FirstOrDefault(x =>
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

    public CourseTopic Add(string name)
    {
        var topic = new CourseTopic(_nextId++, name.Trim());
        _topics.Add(topic);
        return topic;
    }

    public CourseTopic? Update(int id, string name)
    {
        var current = GetById(id);

        if (current is null)
        {
            return null;
        }

        var updated = current with { Name = name.Trim() };
        var index = _topics.IndexOf(current);
        _topics[index] = updated;

        return updated;
    }

    public bool Delete(int id)
    {
        var topic = GetById(id);

        if (topic is null)
        {
            return false;
        }

        _topics.Remove(topic);
        return true;
    }
}
