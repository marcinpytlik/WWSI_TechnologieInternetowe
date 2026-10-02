using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Data;

public sealed class CourseDbContext(DbContextOptions<CourseDbContext> options)
    : DbContext(options)
{
    public DbSet<CourseTopic> Topics => Set<CourseTopic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var topic = modelBuilder.Entity<CourseTopic>();
        topic.HasKey(x => x.Id);
        topic.Property(x => x.Name).HasMaxLength(100).IsRequired();
        topic.HasIndex(x => x.Name).IsUnique();
    }
}
