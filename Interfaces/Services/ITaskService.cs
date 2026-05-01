using TaskManagementSystem.DTOs.Task;

namespace TaskManagementSystem.Interfaces.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskDto>> FilterTasksAsync(TaskFilterDto filter);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
    Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto dto);
    Task<bool> DeleteTaskAsync(int id);
}
