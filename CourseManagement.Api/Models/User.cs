using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

public class User
{
  public int Id { get; set; }

  [Required]
  [MaxLength(100)]
  public string FullName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  [MaxLength(150)]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  [Required]
  public UserRole Role { get; set; }

  public List<Course> TaughtCourses { get; set; } = [];
  public List<Enrollment> Enrollments { get; set; } = [];
}
