using CourseManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Api.Data;

public static class DbInitializer
{
  public static async Task SeedAsync(AppDbContext dbContext)
  {
    await dbContext.Database.EnsureCreatedAsync();

    var adminEmail = "admin@course.local";
    var adminExists = await dbContext.Users.AnyAsync(u => u.Email == adminEmail);
    if (adminExists)
    {
      return;
    }

    dbContext.Users.Add(new User
    {
      FullName = "System Admin",
      Email = adminEmail,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
      Role = UserRole.Admin
    });

    await dbContext.SaveChangesAsync();
  }
}
