using CourseManagement.Api.Models;

namespace CourseManagement.Api.Repositories;

public interface IEnrollmentRepository
{
  IQueryable<Enrollment> Query();
  Task<Enrollment?> GetByIdAsync(int id);
  Task<bool> ExistsAsync(int studentId, int courseId);
  Task AddAsync(Enrollment enrollment);
  Task SaveChangesAsync();
}
