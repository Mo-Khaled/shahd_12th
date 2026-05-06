using CourseManagement.Api.Data;
using CourseManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Repositories;

public class CourseRepository(AppDbContext dbContext) : ICourseRepository
{
  private readonly AppDbContext _dbContext = dbContext;

  public IQueryable<Course> Query() => _dbContext.Courses;

  public Task<Course?> GetByIdAsync(int id)
      => _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id);

  public Task AddAsync(Course course) => _dbContext.Courses.AddAsync(course).AsTask();

  public Task DeleteAsync(Course course)
  {
    _dbContext.Courses.Remove(course);
    return Task.CompletedTask;
  }

  public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
