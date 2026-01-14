using Business.DTOs;
using Business.Services;
using DAL.Services;
using Infra.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Mappers;
using pv179.Models;

namespace pv179.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IVideoService _videoService;
    private readonly IPlaylistService _playlistService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserViewMapper _mapper = new();

    public UserController(ILogger<UserController> logger, IUserService userService, ICurrentUserService currentUserService, IVideoService videoService,
        IPlaylistService playlistService, ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _userService = userService;
        _currentUserService = currentUserService;
        _videoService = videoService;
        _playlistService = playlistService;
        _subscriptionService = subscriptionService;
    }

    public async Task<IActionResult> Index(string? id)
    {
        var targetUserId = id ?? _currentUserService.GetUserId();
        if (targetUserId is null)
        {
            return Unauthorized();
        }

        UserDto? userDto;
        try
        {
            userDto = await _userService.GetByIdAsync(targetUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching creator for user ID {UserId}", targetUserId);
            return StatusCode(500, "Internal server error");
        }

        if (userDto is null)
        {
            return NotFound("User profile not found.");
        }

        var videoFilter = new VideoFilterDto{
            CreatorId = targetUserId,
            PageNumber = 1,
            PageSize = 12
        };

        var playlistFilter = new PlaylistFilterDto {
            CreatorId = targetUserId,
            PageNumber = 1,
            PageSize = 12
        };

        var firstPageVideos = await _videoService.GetByFilterPagedAsync(new VideoFilterDto
        { CreatorId = targetUserId, PageNumber = 1, PageSize = 12 });

        var firstPagePlaylists = await _playlistService.GetByFilterPagedAsync(new PlaylistFilterDto
        { CreatorId = targetUserId, PageNumber = 1, PageSize = 12 });


        var viewModel = _mapper.MapToViewModel(userDto);
        viewModel.Videos = firstPageVideos;
        viewModel.Playlists = firstPagePlaylists;
        viewModel.IsCurrentUser = targetUserId == _currentUserService.GetUserId();
        var isSubscribed = await _subscriptionService.IsUserSubscribedAsync(_currentUserService.GetUserId()!, targetUserId);
        if (isSubscribed.IsError)
        {
            _logger.LogError("Error checking subscription status: {Errors}", string.Join(", ", isSubscribed.Errors.Select(e => e.Description)));
            viewModel.IsSubscribed = false;
        }
        else
        {
            viewModel.IsSubscribed = isSubscribed.Value;
        }

        return View(viewModel);
    }

    #region HTMX Pagination Actions

    [HttpGet]
    public async Task<IActionResult> LoadMoreVideos(string userId, int page = 1)
    {
        var filter = new VideoFilterDto
        {
            CreatorId = userId,
            PageNumber = page,
            PageSize = 12
        };

        var pagedResult = await _videoService.GetByFilterPagedAsync(filter);

        ViewBag.HasMore = page < (int)Math.Ceiling(pagedResult.TotalCount / (double)pagedResult.PageSize);
        ViewBag.NextPage = page + 1;
        ViewBag.UserId = userId;

        return PartialView("_VideoGridItems", pagedResult.Items);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMorePlaylists(string userId, int page = 1)
    {
        var filter = new PlaylistFilterDto
        {
            CreatorId = userId,
            PageNumber = page,
            PageSize = 12
        };

        var pagedResult = await _playlistService.GetByFilterPagedAsync(filter);

        ViewBag.HasMore = page < (int)Math.Ceiling(pagedResult.TotalCount / (double)pagedResult.PageSize);
        ViewBag.NextPage = page + 1;
        ViewBag.UserId = userId;

        return PartialView("_PlaylistGridItems", pagedResult.Items);
    }
    #endregion

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var currentUserId = _currentUserService.GetUserId();
        if (currentUserId == null) return Unauthorized();

        var userDto = await _userService.GetByIdAsync(currentUserId);
        if (userDto == null) return NotFound();

        var editModel = _mapper.MapToEditViewModel(userDto);

        return View(editModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (model.Id != _currentUserService.GetUserId()) return Forbid();

        var userUpdateDto = new UserUpdateDto
        {
            UserName = model.UserName,
            Email = model.Email,
            PricePerMonth = model.PricePerMonth,
            NewPassword = model.NewPassword,
            ConfirmPassword = model.ConfirmPassword
        };

        var result = await _userService.UpdateAsync(model.Id, userUpdateDto);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        return RedirectToAction(nameof(Index), new { id = model.Id });
    }
}
