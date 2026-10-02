using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Data;

public sealed class CourseDbContext(DbContextOptions<CourseDbContext> options)
    : DbContext(options)
{
    public DbSet<CourseTopic> Topics => Set<CourseTopic>();
}
