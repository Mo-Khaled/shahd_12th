using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.DTOs;

public class CreateEnrollmentDto
{
  [Range(1, int.MaxValue)]
  public int CourseId { get; set; }
}

public class AdminEnrollmentDto
{
  [Range(1, int.MaxValue)]
  public int StudentId { get; set; }

  [Range(1, int.MaxValue)]
  public int CourseId { get; set; }
}

public class EnrollmentViewDto
{
  public int EnrollmentId { get; set; }
  public int CourseId { get; set; }
  public string CourseTitle { get; set; } = string.Empty;
  public string CourseDescription { get; set; } = string.Empty;
  public int StudentId { get; set; }
  public string StudentName { get; set; } = string.Empty;
  public DateTime EnrolledAt { get; set; }
}
