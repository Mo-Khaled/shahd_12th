using CourseManagement.Api.DTOs;
using CourseManagement.Api.Extensions;
using CourseManagement.Api.Models;
using CourseManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
  private readonly IEnrollmentService _enrollmentService = enrollmentService;

  [HttpPost]
  [Authorize(Roles = nameof(UserRole.Student))]
  public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var userId = User.GetUserId();
    var result = await _enrollmentService.EnrollAsync(userId, dto);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return Ok(new { message = "Enrolled successfully." });
  }

  [HttpPost("admin")]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> AdminEnroll([FromBody] AdminEnrollmentDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var result = await _enrollmentService.AdminEnrollAsync(dto);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return Ok(new { message = "Student enrolled successfully." });
  }

  [HttpGet("my-courses")]
  [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> GetMyCourses()
  {
    var userId = User.GetUserId();
    var role = Enum.Parse<UserRole>(User.GetUserRole());
    var data = await _enrollmentService.GetMyCoursesAsync(userId, role);
    return Ok(data);
  }

  [HttpGet("course/{courseId:int}")]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> GetByCourse(int courseId)
  {
    var userId = User.GetUserId();
    var role = Enum.Parse<UserRole>(User.GetUserRole());

    var result = await _enrollmentService.GetByCourseAsync(courseId, userId, role);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return Ok(result.Enrollments);
  }

  [HttpGet("all")]
  [Authorize(Roles = nameof(UserRole.Admin))]
  public async Task<IActionResult> GetAll()
  {
    var data = await _enrollmentService.GetAllAsync();
    return Ok(data);
  }
}
