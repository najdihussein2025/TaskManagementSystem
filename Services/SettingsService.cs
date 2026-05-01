using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Dtos.Settings;
using TaskManagementSystem.Interfaces.Services;

namespace TaskManagementSystem.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;

        public SettingsService(AppDbContext context) => _context = context;

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

            // Update full name if provided
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            // Password change — only if user filled in password fields
            bool wantsPasswordChange = !string.IsNullOrWhiteSpace(dto.CurrentPassword)
                                    || !string.IsNullOrWhiteSpace(dto.NewPassword)
                                    || !string.IsNullOrWhiteSpace(dto.ConfirmPassword);

            if (wantsPasswordChange)
            {
                // All 3 fields required
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    return (false, "Current password is required.");

                if (string.IsNullOrWhiteSpace(dto.NewPassword))
                    return (false, "New password is required.");

                if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                    return (false, "Please confirm your new password.");

                // New and confirm must match
                if (dto.NewPassword != dto.ConfirmPassword)
                    return (false, "New password and confirmation do not match.");

                // Minimum length
                if (dto.NewPassword.Length < 6)
                    return (false, "New password must be at least 6 characters.");

                // Verify current password
                // NOTE: if you use BCrypt, replace with:
                // if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                if (user.PasswordHash != dto.CurrentPassword)
                    return (false, "Current password is incorrect.");

                // Save new password
                // NOTE: if you use BCrypt, replace with:
                // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.PasswordHash = dto.NewPassword;
            }

            await _context.SaveChangesAsync();
            return (true, "Profile updated successfully.");
        }
    }
}
