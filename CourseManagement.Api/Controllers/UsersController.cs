using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;
using CourseManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
  private readonly IUserService _userService = userService;

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetAll()
  {
    var users = await _userService.GetAllAsync();
    return Ok(users);
  }

  [HttpPatch("{id:int}/role")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var result = await _userService.UpdateRoleAsync(id, dto);
    if (!result.Success)
    {
      return NotFound(new { message = result.Error });
    }

    return Ok(new { message = "Role updated successfully." });
  }

  [HttpDelete("{id:int}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _userService.DeleteAsync(id);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return NoContent();
  }

  [HttpGet("students")]
  [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
  public async Task<IActionResult> GetStudents()
  {
    var students = await _userService.GetStudentsAsync();
    return Ok(students);
  }

  [HttpGet("instructors")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetInstructors()
  {
    var instructors = await _userService.GetInstructorsAsync();
    return Ok(instructors);
  }

  [HttpPost("create")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var result = await _userService.CreateAsync(dto);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return CreatedAtAction(nameof(GetAll), new { id = result.User?.Id }, result.User);
  }
}
