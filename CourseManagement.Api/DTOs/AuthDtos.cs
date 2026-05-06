using System.ComponentModel.DataAnnotations;
using CourseManagement.Api.Models;

namespace CourseManagement.Api.DTOs;

public class RegisterDto
{
  [Required]
  [MaxLength(100)]
  public string FullName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  public UserRole Role { get; set; } = UserRole.Student;
}

public class LoginDto
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
  public string Token { get; set; } = string.Empty;
  public int UserId { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public UserRole Role { get; set; }
}
