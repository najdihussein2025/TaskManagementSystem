using TaskManagementSystem.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllAsync();//return all users
        Task<ApplicationUser?> GetByIdAsync(int id);//return user by id
        Task<ApplicationUser?> GetByEmailAsync(string email);//return user by email
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
        Task AddAsync(ApplicationUser user);//add user
        Task<Role?> GetRoleByNameAsync(string roleName);//return role by name
        Task<List<ApplicationUser>> GetByStatusAsync(UserStatus status);//return users by status
        Task AddRoleAsync(Role role);//add role
        Task UpdateAsync(ApplicationUser user);//update user
        Task DeleteAsync(int id);//delete user
    }
}