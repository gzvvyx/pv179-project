using Business.DTOs;
using Business.Services;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Models;

namespace pv179.Controllers;

[Authorize]
public class PlaylistController : Controller
{
    private readonly IPlaylistService _playlistService;
    private readonly IVideoService _videoService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(
        IPlaylistService playlistService,
        IVideoService videoService,
        ICurrentUserService currentUserService,
        ILogger<PlaylistController> logger)
    {
        _playlistService = playlistService;
        _videoService = videoService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _currentUserService.GetUserId();
        var playlists = await _playlistService.GetAllAsync();
        var userPlaylists = playlists.Where(p => p.Creator.Id == userId).ToList();

        var viewModel = userPlaylists.Select(p => new PlaylistItemViewModel(
            p.Id,
            p.Name,
            p.Description,
            p.Creator.UserName
        )).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = _currentUserService.GetUserId();
   

        var result = await _playlistService.GetByIdWithVideosAsync(id);
        if (result == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var (playlist, videos) = result.Value;

        var viewModel = new PlaylistDetailViewModel
        {
            Playlist = playlist,
            Videos = videos,
            IsOwner = playlist.Creator.Id == userId
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> AddVideos(int id)
    {
        var userId = _currentUserService.GetUserId();
  

        var playlist = await _playlistService.GetByIdAsync(id);
        if (playlist == null)
        {
            return NotFound();
        }

        if (playlist.Creator.Id != userId)
        {
            return Forbid();
        }

        var allVideos = await _videoService.GetAllAsync();
        var userVideos = allVideos.Where(v => v.Creator.Id == userId).ToList();

        var playlistWithVideos = await _playlistService.GetByIdWithVideosAsync(id);
        var videosInPlaylist = playlistWithVideos?.Item2.Select(v => v.Id).ToList() ?? new List<int>();

        var viewModel = new AddVideosToPlaylistViewModel
        {
            Playlist = playlist,
            UserVideos = userVideos,
            VideosInPlaylist = videosInPlaylist
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVideo(int playlistId, int videoId)
    {
        var userId = _currentUserService.GetUserId();


        var playlist = await _playlistService.GetByIdAsync(playlistId);
        if (playlist == null)
        {
            return NotFound();
        }

        if (playlist.Creator.Id != userId)
        {
            _logger.LogWarning("User {UserId} attempted to add video to playlist {PlaylistId} owned by {OwnerId}",
                userId, playlistId, playlist.Creator.Id);
            return Forbid();
        }

        var result = await _playlistService.AddVideoAsync(playlistId, videoId);
        if (result.IsError)
        {
            _logger.LogError("Failed to add video {VideoId} to playlist {PlaylistId}: {Errors}",
                videoId, playlistId, result.Errors);
        }
        else
        {
            _logger.LogInformation("Video {VideoId} added to playlist {PlaylistId} by user {UserId}",
                videoId, playlistId, userId);
        }

        return RedirectToAction(nameof(AddVideos), new { id = playlistId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string? description)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrWhiteSpace(name))
        {
            return RedirectToAction(nameof(Index));
        }

        if (name.Length > 100)
        {
            return RedirectToAction(nameof(Index));
        }

        if (description != null && description.Length > 500)
        {
            return RedirectToAction(nameof(Index));
        }

        var createDto = new PlaylistCreateDto
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreatorId = userId
        };

        var result = await _playlistService.CreateAsync(createDto);
        if (result.IsError)
        {
            _logger.LogWarning("Failed to create playlist for user {UserId}: {Errors}", userId, result.Errors);
        }
        else
        {
            _logger.LogInformation("Playlist {PlaylistId} created by user {UserId}", result.Value.Id, userId);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVideo(int playlistId, int videoId)
    {
        var userId = _currentUserService.GetUserId();


        var playlist = await _playlistService.GetByIdAsync(playlistId);
        if (playlist == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (playlist.Creator.Id != userId)
        {
            _logger.LogWarning("User {UserId} attempted to remove video from playlist {PlaylistId} owned by {OwnerId}", 
                userId, playlistId, playlist.Creator.Id);
            return RedirectToAction(nameof(Details), new { id = playlistId });
        }

        var result = await _playlistService.RemoveVideoAsync(playlistId, videoId);
        if (result.IsError)
        {
            _logger.LogError("Failed to remove video {VideoId} from playlist {PlaylistId}: {Errors}", 
                videoId, playlistId, result.Errors);
        }
        else
        {
            _logger.LogInformation("Video {VideoId} removed from playlist {PlaylistId} by user {UserId}", 
                videoId, playlistId, userId);
        }

        return RedirectToAction(nameof(Details), new { id = playlistId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _currentUserService.GetUserId();
      


        var playlist = await _playlistService.GetByIdAsync(id);
        if (playlist == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (playlist.Creator.Id != userId)
        {
            _logger.LogWarning("User {UserId} attempted to delete playlist {PlaylistId} owned by {OwnerId}", 
                userId, id, playlist.Creator.Id);
            return RedirectToAction(nameof(Index));
        }

        var result = await _playlistService.DeleteAsync(id);
        if (result.IsError)
        {
            _logger.LogError("Failed to delete playlist {PlaylistId} for user {UserId}: {Errors}", 
                id, userId, result.Errors);
        }
        else
        {
            _logger.LogInformation("Playlist {PlaylistId} deleted by user {UserId}", id, userId);
        }

        return RedirectToAction(nameof(Index));
    }
}