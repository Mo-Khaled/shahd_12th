using CourseManagement.Api.Models;

namespace CourseManagement.Api.Repositories;

public interface IUserRepository
{
  Task<User?> GetByEmailAsync(string email);
  Task<User?> GetByIdAsync(int id);
  IQueryable<User> Query();
  Task AddAsync(User user);
  Task DeleteAsync(User user);
  Task SaveChangesAsync();
}
