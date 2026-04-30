using TaskStatus = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.DTOs.Task;

public class UpdateTaskStatusDto
{
    public int TaskId { get; set; }
    public TaskStatus Status { get; set; }
}
