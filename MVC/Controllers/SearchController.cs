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

    public async Task<IActionResult> All(
        string? query,
        int videoPage = 1,
        int playlistPage = 1,
        int creatorPage = 1,
        int pageSize = 6,
        string? fromDate = null,
        string? toDate = null,
        string? contentSort = null,
        string? creatorSort = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction(nameof(Index));
        }

        var videoFilter = new VideoFilterDto
        {
            Title = query,
            PageNumber = videoPage,
            PageSize = pageSize
        };

        // Apply date filters for videos
        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var parsedFromDate))
        {
            videoFilter.FromDate = parsedFromDate;
        }
        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var parsedToDate))
        {
            videoFilter.ToDate = parsedToDate;
        }

        // Apply content sort for videos (format: "Field-Direction")
        if (!string.IsNullOrEmpty(contentSort))
        {
            var parts = contentSort.Split('-');
            if (parts.Length == 2)
            {
                videoFilter.SortBy = parts[0]; // e.g., "Title", "CreatedAt", "UpdatedAt"
                videoFilter.SortDescending = parts[1] == "Desc";
            }
        }

        var playlistFilter = new PlaylistFilterDto
        {
            Name = query,
            PageNumber = playlistPage,
            PageSize = pageSize
        };

        // Apply date filters for playlists
        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var pFromDate))
        {
            playlistFilter.FromDate = pFromDate;
        }
        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var pToDate))
        {
            playlistFilter.ToDate = pToDate;
        }

        // Apply content sort for playlists (same as videos, but uses "Name" instead of "Title")
        if (!string.IsNullOrEmpty(contentSort))
        {
            var parts = contentSort.Split('-');
            if (parts.Length == 2)
            {
                var sortField = parts[0];
                // Map "Title" to "Name" for playlists
                if (sortField == "Title")
                {
                    sortField = "Name";
                }
                playlistFilter.SortBy = sortField;
                playlistFilter.SortDescending = parts[1] == "Desc";
            }
        }

        var userFilter = new UserFilterDto
        {
            UserName = query,
            PageNumber = creatorPage,
            PageSize = pageSize
        };

        // Apply creator sort (format: "UserName-Asc" or "UserName-Desc")
        if (!string.IsNullOrEmpty(creatorSort))
        {
            var parts = creatorSort.Split('-');
            if (parts.Length == 2)
            {
                userFilter.SortBy = parts[0]; // "UserName"
                userFilter.SortDescending = parts[1] == "Desc";
            }
        }

        var videos = await _videoService.GetByFilterPagedAsync(videoFilter);
        var playlists = await _playlistService.GetByFilterPagedAsync(playlistFilter);
        var creators = await _userService.GetByFilterPagedAsync(userFilter);

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
