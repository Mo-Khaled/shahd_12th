using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.DTOs;

public class CourseDto
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int InstructorId { get; set; }
  public string InstructorName { get; set; } = string.Empty;
}

public class CreateOrUpdateCourseDto
{
  [Required]
  [MaxLength(120)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(400)]
  public string Description { get; set; } = string.Empty;

  [Range(1, int.MaxValue)]
  public int InstructorId { get; set; }
}
