using System.Security.Claims;

namespace TaskManagementSystem.Extensions;

/// <summary>
/// Current user from JWT claims (<see cref="ClaimTypes"/>). No session required.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Numeric user id from <see cref="ClaimTypes.NameIdentifier"/>.</summary>
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
