using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;
using CourseManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Services;

public class CourseService(ICourseRepository courseRepository, IUserRepository userRepository) : ICourseService
{
  private readonly ICourseRepository _courseRepository = courseRepository;
  private readonly IUserRepository _userRepository = userRepository;

  public Task<List<CourseDto>> GetAllAsync()
  {
    return _courseRepository.Query()
        .AsNoTracking()
        .Include(c => c.Instructor)
        .Select(c => new CourseDto
        {
          Id = c.Id,
          Title = c.Title,
          Description = c.Description,
          InstructorId = c.InstructorId,
          InstructorName = c.Instructor.FullName
        })
        .ToListAsync();
  }

  public Task<List<CourseDto>> GetByInstructorAsync(int instructorId)
  {
    return _courseRepository.Query()
        .AsNoTracking()
        .Where(c => c.InstructorId == instructorId)
        .Include(c => c.Instructor)
        .Select(c => new CourseDto
        {
          Id = c.Id,
          Title = c.Title,
          Description = c.Description,
          InstructorId = c.InstructorId,
          InstructorName = c.Instructor.FullName
        })
        .ToListAsync();
  }

  public Task<CourseDto?> GetByIdAsync(int id)
  {
    return _courseRepository.Query()
        .AsNoTracking()
        .Where(c => c.Id == id)
        .Include(c => c.Instructor)
        .Select(c => new CourseDto
        {
          Id = c.Id,
          Title = c.Title,
          Description = c.Description,
          InstructorId = c.InstructorId,
          InstructorName = c.Instructor.FullName
        })
        .FirstOrDefaultAsync();
  }

  public async Task<(bool Success, string Error, CourseDto? Course)> CreateAsync(CreateOrUpdateCourseDto dto, int currentUserId, UserRole currentUserRole)
  {
    if (currentUserRole == UserRole.Student)
    {
      return (false, "Students cannot create courses.", null);
    }

    var instructorId = dto.InstructorId;
    if (currentUserRole == UserRole.Instructor)
    {
      instructorId = currentUserId;
    }

    var instructor = await _userRepository.GetByIdAsync(instructorId);
    if (instructor is null || instructor.Role != UserRole.Instructor)
    {
      return (false, "Selected instructor is invalid.", null);
    }

    var course = new Course
    {
      Title = dto.Title.Trim(),
      Description = dto.Description.Trim(),
      InstructorId = instructorId
    };

    await _courseRepository.AddAsync(course);
    await _courseRepository.SaveChangesAsync();

    return await BuildCourseResult(course.Id);
  }

  public async Task<(bool Success, string Error, CourseDto? Course)> UpdateAsync(int id, CreateOrUpdateCourseDto dto, int currentUserId, UserRole currentUserRole)
  {
    var course = await _courseRepository.GetByIdAsync(id);
    if (course is null)
    {
      return (false, "Course not found.", null);
    }

    if (currentUserRole == UserRole.Student)
    {
      return (false, "Students cannot edit courses.", null);
    }

    if (currentUserRole == UserRole.Instructor && course.InstructorId != currentUserId)
    {
      return (false, "You can only edit your own courses.", null);
    }

    var instructorId = course.InstructorId;
    if (currentUserRole == UserRole.Admin)
    {
      instructorId = dto.InstructorId;
      var instructor = await _userRepository.GetByIdAsync(instructorId);
      if (instructor is null || instructor.Role != UserRole.Instructor)
      {
        return (false, "Selected instructor is invalid.", null);
      }
    }

    course.Title = dto.Title.Trim();
    course.Description = dto.Description.Trim();
    course.InstructorId = instructorId;

    await _courseRepository.SaveChangesAsync();

    return await BuildCourseResult(course.Id);
  }

  public async Task<(bool Success, string Error)> DeleteAsync(int id, int currentUserId, UserRole currentUserRole)
  {
    var course = await _courseRepository.GetByIdAsync(id);
    if (course is null)
    {
      return (false, "Course not found.");
    }

    if (currentUserRole == UserRole.Student)
    {
      return (false, "Students cannot delete courses.");
    }

    if (currentUserRole == UserRole.Instructor && course.InstructorId != currentUserId)
    {
      return (false, "You can only delete your own courses.");
    }

    await _courseRepository.DeleteAsync(course);
    await _courseRepository.SaveChangesAsync();

    return (true, string.Empty);
  }

  private async Task<(bool Success, string Error, CourseDto? Course)> BuildCourseResult(int courseId)
  {
    var created = await GetByIdAsync(courseId);
    return created is null ? (false, "Course operation failed.", null) : (true, string.Empty, created);
  }
}
