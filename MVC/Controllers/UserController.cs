using Business.Services;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.DTOs;
using pv179.Mappers;

namespace pv179.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserViewMapper _mapper = new();

    public UserController(ILogger<UserController> logger, IUserService userService, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _userService = userService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index(string? id)
    {
        var targetUserId = id ?? _currentUserService.GetUserId();
        if (targetUserId is null)
        {
            return Unauthorized();
        }

        UserDetailsDto? userDetailsDto;
        try
        {
            userDetailsDto = await _userService.GetDetailsByIdAsync(targetUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching creator for user ID {UserId}", targetUserId);
            return StatusCode(500, "Internal server error");
        }

        if (userDetailsDto is null)
        {
            return NotFound("User profile not found.");
        }

        var viewModel = _mapper.MapToViewModel(userDetailsDto);
        return View(viewModel);
    }
}
