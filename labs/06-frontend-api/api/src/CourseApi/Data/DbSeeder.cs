using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(CourseDbContext db)
    {
        if (await db.Topics.AnyAsync()) return;

        db.Topics.AddRange(
            new CourseTopic { Name = "HTTP" },
            new CourseTopic { Name = "JavaScript fetch" },
            new CourseTopic { Name = "ASP.NET Core" },
            new CourseTopic { Name = "Entity Framework Core" },
            new CourseTopic { Name = "Docker Compose" });

        await db.SaveChangesAsync();
    }
}
