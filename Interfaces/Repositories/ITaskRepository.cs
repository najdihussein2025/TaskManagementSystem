using TaskManagementSystem.Models;

namespace TaskManagementSystem.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
         Task<TaskItem?> GetByIdAsync(int id);
         Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);
         Task<IEnumerable<TaskItem>> FilterAsync(
            string? search,
            string? status,
            string? priority,
            int? categoryId,
            int? userId
        );
         Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}