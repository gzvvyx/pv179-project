using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class UserUpdateDto
{
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
