using CourseManagement.Api.Data;
using CourseManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
  private readonly AppDbContext _dbContext = dbContext;

  public Task<User?> GetByEmailAsync(string email)
      => _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

  public Task<User?> GetByIdAsync(int id)
      => _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

  public IQueryable<User> Query() => _dbContext.Users;

  public Task AddAsync(User user) => _dbContext.Users.AddAsync(user).AsTask();

  public Task DeleteAsync(User user)
  {
    _dbContext.Users.Remove(user);
    return Task.CompletedTask;
  }

  public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
