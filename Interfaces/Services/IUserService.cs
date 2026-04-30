using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IUserService
    {
    Task CreateAsync(CreateUserDto dto);//create user using dto
        Task<List<UserDto>> GetAllAsync(string? search = null, string? status = null);//get all users with search and status
        Task<UserProfileDto?> GetByIdAsync(int id);//get user by id
        Task UpdateAsync(UpdateUserDto dto);//update user using dto
        Task DeleteAsync(int id);//delete user by id
        Task AssignRoleAsync(AssignRoleDto dto);//assign role to user using dto
        
    }
}