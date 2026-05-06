using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;

namespace CourseManagement.Api.Services;

public interface ICourseService
{
  Task<List<CourseDto>> GetAllAsync();
  Task<List<CourseDto>> GetByInstructorAsync(int instructorId);
  Task<CourseDto?> GetByIdAsync(int id);
  Task<(bool Success, string Error, CourseDto? Course)> CreateAsync(CreateOrUpdateCourseDto dto, int currentUserId, UserRole currentUserRole);
  Task<(bool Success, string Error, CourseDto? Course)> UpdateAsync(int id, CreateOrUpdateCourseDto dto, int currentUserId, UserRole currentUserRole);
  Task<(bool Success, string Error)> DeleteAsync(int id, int currentUserId, UserRole currentUserRole);
}
