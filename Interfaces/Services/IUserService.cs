using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IUserService
    {
    Task CreateAsync(CreateUserDto dto);
        Task<List<UserDto>> GetAllAsync();
        Task<UserProfileDto?> GetByIdAsync(int id);
        Task UpdateAsync(UpdateUserDto dto);
        Task DeleteAsync(int id);
        Task AssignRoleAsync(AssignRoleDto dto);
    }
}