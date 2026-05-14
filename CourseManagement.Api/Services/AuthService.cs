using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;
using CourseManagement.Api.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace CourseManagement.Api.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
  private readonly IUserRepository _userRepository = userRepository;
  private readonly IConfiguration _configuration = configuration;

  public async Task<(bool Success, string Error, AuthTokensDto? Response)> RegisterAsync(RegisterDto dto)
  {
    if (dto.Role == UserRole.Admin)
    {
      return (false, "Registering as Admin is not allowed.", null);
    }

    var email = dto.Email.Trim().ToLowerInvariant();
    var existing = await _userRepository.GetByEmailAsync(email);
    if (existing is not null)
    {
      return (false, "Email is already registered.", null);
    }

    var user = new User
    {
      FullName = dto.FullName.Trim(),
      Email = email,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
      Role = dto.Role
    };

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    return (true, string.Empty, BuildAuthResponse(user));
  }

  public async Task<(bool Success, string Error, AuthTokensDto? Response)> LoginAsync(LoginDto dto)
  {
    var email = dto.Email.Trim().ToLowerInvariant();
    var user = await _userRepository.GetByEmailAsync(email);
    if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
    {
      return (false, "Invalid email or password.", null);
    }

    return (true, string.Empty, BuildAuthResponse(user));
  }

  private AuthTokensDto BuildAuthResponse(User user)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

    var baseClaims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.FullName),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Role, user.Role.ToString())
    };

    var accessClaims = new List<Claim>(baseClaims) { new("typ", "access") };
    var refreshClaims = new List<Claim>(baseClaims) { new("typ", "refresh") };

    var accessTokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(accessClaims),
      Expires = DateTime.UtcNow.AddMinutes(30),
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"],
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var refreshTokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(refreshClaims),
      Expires = DateTime.UtcNow.AddDays(14),
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"],
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
    var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

    return new AuthTokensDto
    {
      AccessToken = tokenHandler.WriteToken(accessToken),
      RefreshToken = tokenHandler.WriteToken(refreshToken),
      UserId = user.Id,
      FullName = user.FullName,
      Email = user.Email,
      Role = user.Role
    };
  }
}
