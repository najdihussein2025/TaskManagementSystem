using System.Security.Claims;

namespace TaskManagementSystem.Extensions;


public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var v = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(v, out var id) ? id : null;
    }

    public static string? GetRole(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Role)?.Value;

    public static string? GetFullName(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Name)?.Value;

    public static bool IsAdmin(this ClaimsPrincipal user) =>
        user.IsInRole("Admin");
}
