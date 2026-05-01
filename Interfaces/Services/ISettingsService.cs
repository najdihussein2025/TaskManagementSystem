using TaskManagementSystem.Dtos.Settings;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<SettingsDto?> GetAdminProfileAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    }
}
