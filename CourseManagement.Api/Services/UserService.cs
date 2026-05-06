using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;
using CourseManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
  private readonly IUserRepository _userRepository = userRepository;

  public Task<List<UserDto>> GetAllAsync()
  {
    return _userRepository.Query()
        .AsNoTracking()
        .OrderBy(u => u.FullName)
        .Select(u => new UserDto
        {
          Id = u.Id,
          FullName = u.FullName,
          Email = u.Email,
          Role = u.Role
        })
        .ToListAsync();
  }

  public async Task<(bool Success, string Error, UserDto? User)> CreateAsync(CreateUserDto dto)
  {
    // Prevent admin creation
    if (dto.Role == UserRole.Admin)
    {
      return (false, "Cannot create Admin users.", null);
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

    var userDto = new UserDto
    {
      Id = user.Id,
      FullName = user.FullName,
      Email = user.Email,
      Role = user.Role
    };

    return (true, string.Empty, userDto);
  }

  public async Task<(bool Success, string Error)> UpdateRoleAsync(int userId, UpdateUserRoleDto dto)
  {
    var user = await _userRepository.GetByIdAsync(userId);
    if (user is null)
    {
      return (false, "User not found.");
    }

    user.Role = dto.Role;
    await _userRepository.SaveChangesAsync();

    return (true, string.Empty);
  }

  public async Task<(bool Success, string Error)> DeleteAsync(int userId)
  {
    var user = await _userRepository.GetByIdAsync(userId);
    if (user is null)
    {
      return (false, "User not found.");
    }

    if (user.Role == UserRole.Admin)
    {
      return (false, "Admin users cannot be deleted.");
    }

    await _userRepository.DeleteAsync(user);
    await _userRepository.SaveChangesAsync();

    return (true, string.Empty);
  }

  public Task<List<UserDto>> GetStudentsAsync()
  {
    return _userRepository.Query()
      .AsNoTracking()
      .Where(u => u.Role == UserRole.Student)
      .OrderBy(u => u.FullName)
      .Select(u => new UserDto
      {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role
      })
      .ToListAsync();
  }

  public Task<List<UserDto>> GetInstructorsAsync()
  {
    return _userRepository.Query()
      .AsNoTracking()
      .Where(u => u.Role == UserRole.Instructor)
      .OrderBy(u => u.FullName)
      .Select(u => new UserDto
      {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role
      })
      .ToListAsync();
  }
}
