using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(CourseDbContext db)
    {
        if (await db.Topics.AnyAsync())
        {
            return;
        }

        db.Topics.AddRange(
            new CourseTopic { Name = "HTTP" },
            new CourseTopic { Name = "HTML, CSS i JavaScript" },
            new CourseTopic { Name = "ASP.NET Core Web API" },
            new CourseTopic { Name = "REST API" },
            new CourseTopic { Name = "Docker" });

        await db.SaveChangesAsync();
    }
}
