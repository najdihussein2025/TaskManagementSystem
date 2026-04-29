namespace TaskManagementSystem.DTOs.User;

public class UserProfileDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? RoleName { get; set; }
    public DateTime CreatedAt { get; set; }
}
