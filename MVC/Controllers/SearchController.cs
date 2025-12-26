using Business.DTOs;
using Business.Services;
using Infra.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Models;

namespace pv179.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly ILogger<SearchController> _logger;
    private readonly IVideoService _videoService;
    private readonly IPlaylistService _playlistService;
    private readonly IUserService _userService;

    public SearchController(
        ILogger<SearchController> logger,
        IVideoService videoService,
        IPlaylistService playlistService,
        IUserService userService)
    {
        _logger = logger;
        _videoService = videoService;
        _playlistService = playlistService;
        _userService = userService;
    }

    public IActionResult Index(string? query, string? category)
    {
        var model = new SearchViewModel
        {
            Query = query ?? string.Empty,
            Category = category ?? "all"
        };

        return View(model);
    }

    public async Task<IActionResult> Videos(string? query)
    {
        var filter = new VideoFilterDto
        {
            Title = query
        };

        var videos = await _videoService.GetByFilterAsync(filter);

        var model = new SearchResultsViewModel<VideoDto>
        {
            Query = query ?? string.Empty,
            Results = videos,
            Category = "videos"
        };

        return View("Results", model);
    }

    public async Task<IActionResult> Playlists(string? query)
    {
        var filter = new PlaylistFilterDto
        {
            Name = query
        };

        var playlists = await _playlistService.GetByFilterAsync(filter);

        var model = new SearchResultsViewModel<PlaylistDto>
        {
            Query = query ?? string.Empty,
            Results = playlists,
            Category = "playlists"
        };

        return View("Results", model);
    }

    public async Task<IActionResult> Creators(string? query)
    {
        var filter = new UserFilterDto
        {
            UserName = query
        };

        var users = await _userService.GetByFilterAsync(filter);

        var model = new SearchResultsViewModel<UserDto>
        {
            Query = query ?? string.Empty,
            Results = users,
            Category = "creators"
        };

        return View("Results", model);
    }

    public async Task<IActionResult> All(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction(nameof(Index));
        }

        var videoFilter = new VideoFilterDto
        {
            Title = query
        };

        var playlistFilter = new PlaylistFilterDto
        {
            Name = query
        };

        var userFilter = new UserFilterDto
        {
            UserName = query
        };

        var videos = await _videoService.GetByFilterAsync(videoFilter);
        var playlists = await _playlistService.GetByFilterAsync(playlistFilter);
        var creators = await _userService.GetByFilterAsync(userFilter);

        var model = new SearchAllResultsViewModel
        {
            Query = query,
            Videos = videos,
            Playlists = playlists,
            Creators = creators
        };

        return View(model);
    }
}
