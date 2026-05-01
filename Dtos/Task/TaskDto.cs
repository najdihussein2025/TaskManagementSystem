using TaskPriority = TaskManagementSystem.Enums.TaskPriority;
using TaskStatus = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.DTOs.Task;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
