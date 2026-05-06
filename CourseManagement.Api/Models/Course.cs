using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

public class Course
{
  public int Id { get; set; }

  [Required]
  [MaxLength(120)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(400)]
  public string Description { get; set; } = string.Empty;

  public int InstructorId { get; set; }
  public User Instructor { get; set; } = null!;

  public List<Enrollment> Enrollments { get; set; } = [];
}
