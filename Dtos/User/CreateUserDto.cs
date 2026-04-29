namespace TaskManagementSystem.DTOs.User;

public class CreateUserDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? RoleName { get; set; }
    public string? Status { get; set; }
}
