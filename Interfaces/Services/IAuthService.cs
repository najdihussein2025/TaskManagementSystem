using TaskManagementSystem.DTOs.Auth;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<bool> LoginAsync(LoginDto dto);
        Task LogoutAsync();
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
    }
}