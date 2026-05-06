using CourseManagement.Api.DTOs;

namespace CourseManagement.Api.Services;

public interface IAuthService
{
  Task<(bool Success, string Error, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto);
  Task<(bool Success, string Error, AuthResponseDto? Response)> LoginAsync(LoginDto dto);
}
