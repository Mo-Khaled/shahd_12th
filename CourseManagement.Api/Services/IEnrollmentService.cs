using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;

namespace CourseManagement.Api.Services;

public interface IEnrollmentService
{
  Task<(bool Success, string Error)> EnrollAsync(int studentId, CreateEnrollmentDto dto);
  Task<(bool Success, string Error)> AdminEnrollAsync(AdminEnrollmentDto dto);
  Task<List<EnrollmentViewDto>> GetMyCoursesAsync(int currentUserId, UserRole currentUserRole);
  Task<(bool Success, string Error, List<EnrollmentViewDto>? Enrollments)> GetByCourseAsync(int courseId, int currentUserId, UserRole currentUserRole);
  Task<List<EnrollmentViewDto>> GetAllAsync();
}
