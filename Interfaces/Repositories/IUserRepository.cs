using TaskManagementSystem.Models;

namespace TaskManagementSystem.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(int id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task AddAsync(ApplicationUser user);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task AddRoleAsync(Role role);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(int id);
    }
}