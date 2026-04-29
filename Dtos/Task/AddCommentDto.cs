namespace TaskManagementSystem.DTOs.Task;

public class AddCommentDto
{
    public int TaskId { get; set; }
    public string? CommentText { get; set; }
}
