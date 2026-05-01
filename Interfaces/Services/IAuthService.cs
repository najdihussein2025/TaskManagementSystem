using TaskManagementSystem.DTOs.Auth;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, string? Role, string? Error)> LoginAsync(LoginDto dto);
        Task<(bool Success, string? Token, string? Role, string? Error)> GoogleLoginAsync(string googleId, string email, string fullName);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
