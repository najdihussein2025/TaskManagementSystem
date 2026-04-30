using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Description { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}
