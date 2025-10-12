using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class UserCreateDto
{
    [Required]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
