using TaskManagementSystem.DTOs.Task;
using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IEnumerable<TaskDto>> FilterTasksAsync(TaskFilterDto filter)
    {
        var tasks = await _taskRepository.FilterAsync(
            filter.Search,
            filter.Status,
            filter.Priority,
            filter.CategoryId,
            filter.UserId);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
    {
        var fallbackUserId = await GetRequiredUserIdAsync();
        var assignedUserId = await TryGetValidUserIdAsync(dto.AssignedToUserId) ?? fallbackUserId;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description ?? string.Empty,
            AssignedToUserId = assignedUserId,
            CategoryId = dto.CategoryId ?? 0,
            Priority = dto.Priority,
            DueDate = dto.DueDate ?? default,
            Status = dto.Status,
            CreatedByUserId = fallbackUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);

        var created = await _taskRepository.GetByIdAsync(task.Id);
        return MapToDto(created!);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return null;
        var assignedUserId = await TryGetValidUserIdAsync(dto.AssignedToUserId);

        task.Title = dto.Title;
        task.Description = dto.Description ?? string.Empty;
        task.AssignedToUserId = assignedUserId ?? task.AssignedToUserId;
        task.CategoryId = dto.CategoryId ?? 0;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate ?? default;
        task.Status = dto.Status;

        await _taskRepository.UpdateAsync(task);

        var updated = await _taskRepository.GetByIdAsync(id);
        return MapToDto(updated!);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        if (!await _taskRepository.ExistsAsync(id)) return false;
        await _taskRepository.DeleteAsync(id);
        return true;
    }

    private async Task<int?> TryGetValidUserIdAsync(int? userId)
    {
        if (userId.HasValue)
        {
            var existing = await _userRepository.GetByIdAsync(userId.Value);
            if (existing != null) return existing.Id;
        }
        return null;
    }

    private async Task<int> GetRequiredUserIdAsync()
    {
        var users = await _userRepository.GetAllAsync();
        if (users.Count == 0)
            throw new InvalidOperationException("No users exist to assign task ownership.");

        return users[0].Id;
    }

    private static TaskDto MapToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        AssignedToUserId = task.AssignedToUserId,
        AssignedToName = task.AssignedToUser?.FullName,
        CategoryId = task.CategoryId,
        CategoryName = task.Category?.Name,
        Priority = task.Priority,
        DueDate = task.DueDate,
        Status = task.Status,
        CreatedAt = task.CreatedAt
    };
}
