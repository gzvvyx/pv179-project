using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class UserUpdateDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
    public decimal? PricePerMonth { get; set; }
}
