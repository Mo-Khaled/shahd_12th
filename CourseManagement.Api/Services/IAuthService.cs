using CourseManagement.Api.DTOs;

namespace CourseManagement.Api.Services;

public interface IAuthService
{
  Task<(bool Success, string Error, AuthTokensDto? Response)> RegisterAsync(RegisterDto dto);
  Task<(bool Success, string Error, AuthTokensDto? Response)> LoginAsync(LoginDto dto);
}
