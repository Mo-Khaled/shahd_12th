using System.ComponentModel.DataAnnotations;
using CourseManagement.Api.Models;

namespace CourseManagement.Api.DTOs;

public class UserDto
{
  public int Id { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public UserRole Role { get; set; }
}

public class UpdateUserRoleDto
{
  [Required]
  public UserRole Role { get; set; }
}

public class CreateUserDto
{
  [Required]
  [StringLength(200)]
  public string FullName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(100, MinimumLength = 6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  public UserRole Role { get; set; }
}
