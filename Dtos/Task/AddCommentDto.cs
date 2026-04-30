using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Task;

public class AddCommentDto
{
    public int TaskId { get; set; }

    [Required]
    [MaxLength(500)]
    public string CommentText { get; set; } = string.Empty;
}
