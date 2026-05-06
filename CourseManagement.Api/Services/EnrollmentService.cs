using CourseManagement.Api.DTOs;
using CourseManagement.Api.Models;
using CourseManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Services;

public class EnrollmentService(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository) : IEnrollmentService
{
  private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
  private readonly ICourseRepository _courseRepository = courseRepository;

  public async Task<(bool Success, string Error)> EnrollAsync(int studentId, CreateEnrollmentDto dto)
  {
    var courseExists = await _courseRepository.Query()
        .AsNoTracking()
        .AnyAsync(c => c.Id == dto.CourseId);

    if (!courseExists)
    {
      return (false, "Course not found.");
    }

    var alreadyEnrolled = await _enrollmentRepository.ExistsAsync(studentId, dto.CourseId);
    if (alreadyEnrolled)
    {
      return (false, "You are already enrolled in this course.");
    }

    await _enrollmentRepository.AddAsync(new Enrollment
    {
      StudentId = studentId,
      CourseId = dto.CourseId,
      EnrolledAt = DateTime.UtcNow
    });

    await _enrollmentRepository.SaveChangesAsync();
    return (true, string.Empty);
  }

  public async Task<(bool Success, string Error)> AdminEnrollAsync(AdminEnrollmentDto dto)
  {
    var courseExists = await _courseRepository.Query()
        .AsNoTracking()
        .AnyAsync(c => c.Id == dto.CourseId);

    if (!courseExists)
    {
      return (false, "Course not found.");
    }

    var studentExists = await _courseRepository.Query()
        .AsNoTracking()
        .AnyAsync(c => c.Id == dto.CourseId);

    if (!studentExists)
    {
      return (false, "Student not found.");
    }

    var alreadyEnrolled = await _enrollmentRepository.ExistsAsync(dto.StudentId, dto.CourseId);
    if (alreadyEnrolled)
    {
      return (false, "Student is already enrolled in this course.");
    }

    await _enrollmentRepository.AddAsync(new Enrollment
    {
      StudentId = dto.StudentId,
      CourseId = dto.CourseId,
      EnrolledAt = DateTime.UtcNow
    });

    await _enrollmentRepository.SaveChangesAsync();
    return (true, string.Empty);
  }

  public Task<List<EnrollmentViewDto>> GetMyCoursesAsync(int currentUserId, UserRole currentUserRole)
  {
    if (currentUserRole == UserRole.Student)
    {
      return _enrollmentRepository.Query()
          .AsNoTracking()
          .Where(e => e.StudentId == currentUserId)
          .Include(e => e.Course)
          .Include(e => e.Student)
          .Select(e => new EnrollmentViewDto
          {
            EnrollmentId = e.Id,
            CourseId = e.CourseId,
            CourseTitle = e.Course.Title,
            CourseDescription = e.Course.Description,
            StudentId = e.StudentId,
            StudentName = e.Student.FullName,
            EnrolledAt = e.EnrolledAt
          })
          .ToListAsync();
    }

    return _enrollmentRepository.Query()
        .AsNoTracking()
        .Where(e => e.Course.InstructorId == currentUserId)
        .Include(e => e.Course)
        .Include(e => e.Student)
        .Select(e => new EnrollmentViewDto
        {
          EnrollmentId = e.Id,
          CourseId = e.CourseId,
          CourseTitle = e.Course.Title,
          CourseDescription = e.Course.Description,
          StudentId = e.StudentId,
          StudentName = e.Student.FullName,
          EnrolledAt = e.EnrolledAt
        })
        .ToListAsync();
  }

  public async Task<(bool Success, string Error, List<EnrollmentViewDto>? Enrollments)> GetByCourseAsync(int courseId, int currentUserId, UserRole currentUserRole)
  {
    if (currentUserRole == UserRole.Student)
    {
      return (false, "Students cannot access this endpoint.", null);
    }

    if (currentUserRole == UserRole.Instructor)
    {
      var ownsCourse = await _courseRepository.Query()
          .AsNoTracking()
          .AnyAsync(c => c.Id == courseId && c.InstructorId == currentUserId);

      if (!ownsCourse)
      {
        return (false, "You can only view enrollments in your own courses.", null);
      }
    }

    var enrollments = await _enrollmentRepository.Query()
        .AsNoTracking()
        .Where(e => e.CourseId == courseId)
        .Include(e => e.Course)
        .Include(e => e.Student)
        .Select(e => new EnrollmentViewDto
        {
          EnrollmentId = e.Id,
          CourseId = e.CourseId,
          CourseTitle = e.Course.Title,
          CourseDescription = e.Course.Description,
          StudentId = e.StudentId,
          StudentName = e.Student.FullName,
          EnrolledAt = e.EnrolledAt
        })
        .ToListAsync();

    return (true, string.Empty, enrollments);
  }

  public Task<List<EnrollmentViewDto>> GetAllAsync()
  {
    return _enrollmentRepository.Query()
      .AsNoTracking()
      .Include(e => e.Course)
      .Include(e => e.Student)
      .OrderByDescending(e => e.EnrolledAt)
      .Select(e => new EnrollmentViewDto
      {
        EnrollmentId = e.Id,
        CourseId = e.CourseId,
        CourseTitle = e.Course.Title,
        CourseDescription = e.Course.Description,
        StudentId = e.StudentId,
        StudentName = e.Student.FullName,
        EnrolledAt = e.EnrolledAt
      })
      .ToListAsync();
  }
}
