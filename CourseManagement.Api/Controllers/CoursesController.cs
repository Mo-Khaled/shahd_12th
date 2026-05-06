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
public class CoursesController(ICourseService courseService) : ControllerBase
{
  private readonly ICourseService _courseService = courseService;

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var courses = await _courseService.GetAllAsync();
    return Ok(courses);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var course = await _courseService.GetByIdAsync(id);
    return course is null ? NotFound(new { message = "Course not found." }) : Ok(course);
  }

  [HttpGet("my")]
  [Authorize(Roles = nameof(UserRole.Instructor))]
  public async Task<IActionResult> GetMyCourses()
  {
    var userId = User.GetUserId();
    var courses = await _courseService.GetByInstructorAsync(userId);
    return Ok(courses);
  }

  [HttpPost]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> Create([FromBody] CreateOrUpdateCourseDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var userId = User.GetUserId();
    var role = Enum.Parse<UserRole>(User.GetUserRole());
    var result = await _courseService.CreateAsync(dto, userId, role);

    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return CreatedAtAction(nameof(GetById), new { id = result.Course!.Id }, result.Course);
  }

  [HttpPut("{id:int}")]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateCourseDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var userId = User.GetUserId();
    var role = Enum.Parse<UserRole>(User.GetUserRole());
    var result = await _courseService.UpdateAsync(id, dto, userId, role);

    if (!result.Success)
    {
      return result.Error == "Course not found."
          ? NotFound(new { message = result.Error })
          : BadRequest(new { message = result.Error });
    }

    return Ok(result.Course);
  }

  [HttpDelete("{id:int}")]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> Delete(int id)
  {
    var userId = User.GetUserId();
    var role = Enum.Parse<UserRole>(User.GetUserRole());
    var result = await _courseService.DeleteAsync(id, userId, role);

    if (!result.Success)
    {
      return result.Error == "Course not found."
          ? NotFound(new { message = result.Error })
          : BadRequest(new { message = result.Error });
    }

    return NoContent();
  }
}
