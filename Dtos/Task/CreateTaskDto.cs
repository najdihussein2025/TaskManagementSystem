using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Task;

public class CreateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public int CategoryId { get; set; }
    public int AssignedToUserId { get; set; }
}
