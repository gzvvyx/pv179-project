using Business.DTOs;

namespace MVC.Areas.Admin.Models;

public class UserWithRolesViewModel
{
    public UserDto User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

