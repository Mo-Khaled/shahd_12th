using System.Security.Claims;

namespace CourseManagement.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static int GetUserId(this ClaimsPrincipal user)
  {
    var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return int.TryParse(value, out var id) ? id : 0;
  }

  public static string GetUserRole(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
  }
}
