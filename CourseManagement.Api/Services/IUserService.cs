using CourseManagement.Api.DTOs;

namespace CourseManagement.Api.Services;

public interface IUserService
{
  Task<List<UserDto>> GetAllAsync();
  Task<List<UserDto>> GetStudentsAsync();
  Task<List<UserDto>> GetInstructorsAsync();
  Task<(bool Success, string Error, UserDto? User)> CreateAsync(CreateUserDto dto);
  Task<(bool Success, string Error)> UpdateRoleAsync(int userId, UpdateUserRoleDto dto);
  Task<(bool Success, string Error)> DeleteAsync(int userId);
}
