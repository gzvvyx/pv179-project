using System.ComponentModel.DataAnnotations;
using Business.DTOs;

namespace MVC.Areas.Admin.Models;

public class EditUserViewModel
{
    public UserDto User { get; set; } = null!;
    public List<string> SelectedRoles { get; set; } = new();
    public List<RoleCheckboxItem> AvailableRoles { get; set; } = new();
    
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string? NewPassword { get; set; }
    
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
    
    [Display(Name = "Price Per Month")]
    public decimal? PricePerMonth { get; set; }
}

public class RoleCheckboxItem
{
    public string RoleName { get; set; } = null!;
    public bool IsSelected { get; set; }
}

