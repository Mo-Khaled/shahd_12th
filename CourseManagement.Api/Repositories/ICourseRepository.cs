using CourseManagement.Api.Models;

namespace CourseManagement.Api.Repositories;

public interface ICourseRepository
{
  IQueryable<Course> Query();
  Task<Course?> GetByIdAsync(int id);
  Task AddAsync(Course course);
  Task DeleteAsync(Course course);
  Task SaveChangesAsync();
}
