using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.User;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public UserStatus Status { get; set; }
}
