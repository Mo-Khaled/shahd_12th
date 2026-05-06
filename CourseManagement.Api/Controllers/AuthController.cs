using CourseManagement.Api.DTOs;
using CourseManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var result = await _authService.RegisterAsync(dto);
    if (!result.Success)
    {
      return BadRequest(new { message = result.Error });
    }

    return Ok(result.Response);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto dto)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    var result = await _authService.LoginAsync(dto);
    if (!result.Success)
    {
      return Unauthorized(new { message = result.Error });
    }

    return Ok(result.Response);
  }
}
