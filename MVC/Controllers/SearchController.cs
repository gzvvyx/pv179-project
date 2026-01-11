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
    private readonly ICategoryService _categoryService;

    public SearchController(
        ILogger<SearchController> logger,
        IVideoService videoService,
        IPlaylistService playlistService,
        IUserService userService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _videoService = videoService;
        _playlistService = playlistService;
        _userService = userService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string? query, string? category)
    {
        var categories = await _categoryService.GetAllAsync();
        
        var model = new SearchViewModel
        {
            Query = query ?? string.Empty,
            Category = category ?? "all",
            AvailableCategories = categories
        };

        return View(model);
    }

    public async Task<IActionResult> All(
        string? query,
        int? categoryId = null,
        List<int>? categoryIds = null,
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

        // Combine single categoryId with categoryIds list
        var selectedCategoryIds = new List<int>();
        if (categoryIds != null && categoryIds.Any())
        {
            selectedCategoryIds.AddRange(categoryIds);
        }
        if (categoryId.HasValue && !selectedCategoryIds.Contains(categoryId.Value))
        {
            selectedCategoryIds.Add(categoryId.Value);
        }

        var videoFilter = new VideoFilterDto
        {
            Title = query,
            PageNumber = videoPage,
            PageSize = pageSize,
            CategoryId = categoryId,
            CategoryIds = selectedCategoryIds.Any() ? selectedCategoryIds : null
        };

        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var parsedFromDate))
        {
            videoFilter.FromDate = parsedFromDate;
        }
        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var parsedToDate))
        {
            videoFilter.ToDate = parsedToDate;
        }

        if (!string.IsNullOrEmpty(contentSort))
        {
            var parts = contentSort.Split('-');
            if (parts.Length == 2)
            {
                videoFilter.SortBy = parts[0];
                videoFilter.SortDescending = parts[1] == "Desc";
            }
        }

        var playlistFilter = new PlaylistFilterDto
        {
            Name = query,
            PageNumber = playlistPage,
            PageSize = pageSize
        };

        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var pFromDate))
        {
            playlistFilter.FromDate = pFromDate;
        }
        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var pToDate))
        {
            playlistFilter.ToDate = pToDate;
        }

        if (!string.IsNullOrEmpty(contentSort))
        {
            var parts = contentSort.Split('-');
            if (parts.Length == 2)
            {
                var sortField = parts[0];
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

        if (!string.IsNullOrEmpty(creatorSort))
        {
            var parts = creatorSort.Split('-');
            if (parts.Length == 2)
            {
                userFilter.SortBy = parts[0];
                userFilter.SortDescending = parts[1] == "Desc";
            }
        }

        var videos = await _videoService.GetByFilterPagedAsync(videoFilter);
        var playlists = await _playlistService.GetByFilterPagedAsync(playlistFilter);
        var creators = await _userService.GetByFilterPagedAsync(userFilter);
        var categories = await _categoryService.GetAllAsync();

        var model = new SearchAllResultsViewModel
        {
            Query = query,
            Videos = videos,
            Playlists = playlists,
            Creators = creators,
            AvailableCategories = categories,
            SelectedCategoryIds = selectedCategoryIds
        };

        return View(model);
    }
}
