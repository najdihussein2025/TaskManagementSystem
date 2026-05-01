using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Dtos.Settings;
using TaskManagementSystem.DTOs.Auth;
using TaskManagementSystem.Interfaces.Services;

namespace TaskManagementSystem.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public SettingsService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<SettingsDto?> GetAdminProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new SettingsDto
            {
                FullName = user.FullName,
                Email = user.Email
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(
            int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

 
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();


            bool wantsPasswordChange = !string.IsNullOrWhiteSpace(dto.CurrentPassword)
                                    || !string.IsNullOrWhiteSpace(dto.NewPassword)
                                    || !string.IsNullOrWhiteSpace(dto.ConfirmPassword);

            if (wantsPasswordChange)
            {
                var changeDto = new ChangePasswordDto
                {
                    CurrentPassword = dto.CurrentPassword,
                    NewPassword = dto.NewPassword,
                    ConfirmPassword = dto.ConfirmPassword
                };

                var (pwOk, pwError) = await _authService.ChangePasswordAsync(userId, changeDto);
                if (!pwOk)
                    return (false, pwError ?? "Could not update password.");
            }

            await _context.SaveChangesAsync();
            return (true, "Profile updated successfully.");
        }
    }
}
