using CourseManagement.Api.Data;
using CourseManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Repositories;

public class EnrollmentRepository(AppDbContext dbContext) : IEnrollmentRepository
{
  private readonly AppDbContext _dbContext = dbContext;

  public IQueryable<Enrollment> Query() => _dbContext.Enrollments;

  public Task<Enrollment?> GetByIdAsync(int id)
      => _dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == id);

  public Task<bool> ExistsAsync(int studentId, int courseId)
      => _dbContext.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

  public Task AddAsync(Enrollment enrollment) => _dbContext.Enrollments.AddAsync(enrollment).AsTask();

  public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
