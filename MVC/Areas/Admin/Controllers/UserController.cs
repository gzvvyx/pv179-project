using Business.DTOs;
using Business.Services;
using DAL.Authorization;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Admin.Factories;
using MVC.Areas.Admin.Models;
using MVC.Extensions;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly IEditUserViewModelFactory _viewModelFactory;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        UserManager<User> userManager,
        IEditUserViewModelFactory viewModelFactory,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _userManager = userManager;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    [Route("", Name = "AdminUsers")]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();

        var usersWithRoles = new List<UserWithRolesViewModel>();
        foreach (var user in users)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser != null)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }
        }

        return View(usersWithRoles);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = await _viewModelFactory.CreateAsync(user);
        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userUpdateDto = new UserUpdateDto
        {
            Roles = model.SelectedRoles,
            NewPassword = model.NewPassword,
            ConfirmPassword = model.ConfirmPassword
        };

        var result = await _userService.UpdateAsync(id, userUpdateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            await _viewModelFactory.PopulateViewModelAsync(model, user);
            return View(model);
        }

        TempData["SuccessMessage"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}

