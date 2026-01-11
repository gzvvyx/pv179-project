using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class CategoryCreateDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
}

