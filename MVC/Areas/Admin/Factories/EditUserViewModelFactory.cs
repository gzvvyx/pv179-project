using DAL.Authorization;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditUserViewModelFactory : IEditUserViewModelFactory
{
    private readonly UserManager<User> _userManager;

    public EditUserViewModelFactory(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<EditUserViewModel> CreateAsync(Business.DTOs.UserDto user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        var currentRoles = identityUser != null 
            ? (await _userManager.GetRolesAsync(identityUser)).ToList() 
            : new List<string>();

        var model = new EditUserViewModel
        {
            User = user,
            SelectedRoles = currentRoles,
            AvailableRoles = CreateRoleCheckboxes(currentRoles),
            PricePerMonth = user.PricePerMonth
        };

        return model;
    }

    public async Task PopulateViewModelAsync(EditUserViewModel model, Business.DTOs.UserDto user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        var currentRoles = identityUser != null 
            ? (await _userManager.GetRolesAsync(identityUser)).ToList() 
            : new List<string>();

        model.User = user;
        model.AvailableRoles = CreateRoleCheckboxes(model.SelectedRoles);
    }

    private List<RoleCheckboxItem> CreateRoleCheckboxes(List<string> selectedRoles)
    {
        var allRoles = new[] { AppRoles.Admin, AppRoles.User };
        
        return allRoles.Select(role => new RoleCheckboxItem
        {
            RoleName = role,
            IsSelected = selectedRoles.Contains(role)
        }).ToList();
    }
}

