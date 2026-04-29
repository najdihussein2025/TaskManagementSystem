using TaskManagementSystem.DTOs.Task;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllAsync();
        Task<TaskDetailsDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateTaskDto dto);
        Task UpdateAsync(UpdateTaskDto dto);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(UpdateTaskStatusDto dto);
        Task AddCommentAsync(AddCommentDto dto);
    }
}