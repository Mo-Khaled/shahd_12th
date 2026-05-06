using CourseManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<User> Users => Set<User>();
  public DbSet<Course> Courses => Set<Course>();
  public DbSet<Enrollment> Enrollments => Set<Enrollment>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

    modelBuilder.Entity<Course>()
        .HasOne(c => c.Instructor)
        .WithMany(u => u.TaughtCourses)
        .HasForeignKey(c => c.InstructorId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.Student)
        .WithMany(u => u.Enrollments)
        .HasForeignKey(e => e.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.Course)
        .WithMany(c => c.Enrollments)
        .HasForeignKey(e => e.CourseId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Enrollment>()
        .HasIndex(e => new { e.StudentId, e.CourseId })
        .IsUnique();
  }
}
