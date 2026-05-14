using CourseManagement.Api.DTOs;
using CourseManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IConfiguration configuration, IWebHostEnvironment env) : ControllerBase
{
  private readonly IAuthService _authService = authService;
  private readonly IConfiguration _configuration = configuration;
  private readonly IWebHostEnvironment _env = env;

  private const string AccessCookieName = "cms_access";
  private const string RefreshCookieName = "cms_refresh";

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

    SetAuthCookies(result.Response!);
    return Ok(ToSession(result.Response!));
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

    SetAuthCookies(result.Response!);
    return Ok(ToSession(result.Response!));
  }

  [HttpPost("refresh")]
  public IActionResult Refresh()
  {
    var refreshToken = Request.Cookies[RefreshCookieName];
    if (string.IsNullOrWhiteSpace(refreshToken))
    {
      return Unauthorized(new { message = "Missing refresh token." });
    }

    var principal = ValidateToken(refreshToken);
    if (principal is null || principal.FindFirstValue("typ") != "refresh")
    {
      return Unauthorized(new { message = "Invalid refresh token." });
    }

    var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    var fullName = principal.FindFirstValue(ClaimTypes.Name);
    var email = principal.FindFirstValue(ClaimTypes.Email);
    var role = principal.FindFirstValue(ClaimTypes.Role);

    if (!int.TryParse(userId, out var parsedUserId) ||
        string.IsNullOrWhiteSpace(fullName) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(role) ||
        !Enum.TryParse(role, out CourseManagement.Api.Models.UserRole parsedRole))
    {
      return Unauthorized(new { message = "Invalid refresh token claims." });
    }

    // We don't hit the database here; we mint new tokens from the refresh claims.
    var tokens = new AuthTokensDto
    {
      AccessToken = MintToken(parsedUserId, fullName, email, parsedRole.ToString(), "access", DateTime.UtcNow.AddMinutes(30)),
      RefreshToken = MintToken(parsedUserId, fullName, email, parsedRole.ToString(), "refresh", DateTime.UtcNow.AddDays(14)),
      UserId = parsedUserId,
      FullName = fullName,
      Email = email,
      Role = parsedRole
    };

    SetAuthCookies(tokens);
    return Ok(ToSession(tokens));
  }

  [Authorize]
  [HttpGet("me")]
  public IActionResult Me()
  {
    return Ok(new AuthSessionDto
    {
      UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
      FullName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
      Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
      Role = Enum.Parse<CourseManagement.Api.Models.UserRole>(User.FindFirstValue(ClaimTypes.Role) ?? "Student")
    });
  }

  [HttpPost("logout")]
  public IActionResult Logout()
  {
    ClearAuthCookies();
    return NoContent();
  }

  private void SetAuthCookies(AuthTokensDto tokens)
  {
    var baseOptions = new CookieOptions
    {
      HttpOnly = true,
      SameSite = SameSiteMode.Lax,
      Secure = !_env.IsDevelopment(),
      Path = "/"
    };

    var accessOptions = new CookieOptions
    {
      HttpOnly = baseOptions.HttpOnly,
      SameSite = baseOptions.SameSite,
      Secure = baseOptions.Secure,
      Path = baseOptions.Path,
      Expires = DateTimeOffset.UtcNow.AddMinutes(30)
    };

    var refreshOptions = new CookieOptions
    {
      HttpOnly = baseOptions.HttpOnly,
      SameSite = baseOptions.SameSite,
      Secure = baseOptions.Secure,
      Path = baseOptions.Path,
      Expires = DateTimeOffset.UtcNow.AddDays(14)
    };

    Response.Cookies.Append(AccessCookieName, tokens.AccessToken, accessOptions);
    Response.Cookies.Append(RefreshCookieName, tokens.RefreshToken, refreshOptions);
  }

  private void ClearAuthCookies()
  {
    Response.Cookies.Delete(AccessCookieName, new CookieOptions { Path = "/" });
    Response.Cookies.Delete(RefreshCookieName, new CookieOptions { Path = "/" });
  }

  private static AuthSessionDto ToSession(AuthTokensDto tokens) => new()
  {
    UserId = tokens.UserId,
    FullName = tokens.FullName,
    Email = tokens.Email,
    Role = tokens.Role
  };

  private ClaimsPrincipal? ValidateToken(string token)
  {
    var jwtKey = _configuration["Jwt:Key"];
    if (string.IsNullOrWhiteSpace(jwtKey))
    {
      return null;
    }

    var handler = new JwtSecurityTokenHandler();
    try
    {
      return handler.ValidateToken(token, new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _configuration["Jwt:Issuer"],
        ValidAudience = _configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
      }, out _);
    }
    catch
    {
      return null;
    }
  }

  private string MintToken(int userId, string fullName, string email, string role, string typ, DateTime expiresUtc)
  {
    var handler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, userId.ToString()),
      new(ClaimTypes.Name, fullName),
      new(ClaimTypes.Email, email),
      new(ClaimTypes.Role, role),
      new("typ", typ)
    };

    var descriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = expiresUtc,
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"],
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = handler.CreateToken(descriptor);
    return handler.WriteToken(token);
  }
}
