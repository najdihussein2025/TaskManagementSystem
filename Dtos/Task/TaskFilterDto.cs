namespace TaskManagementSystem.DTOs.Task;

public class TaskFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
}
