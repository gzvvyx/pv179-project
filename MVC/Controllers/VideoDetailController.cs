using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Services;
using Business.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;
using MVC.Extensions;
using MVC.Models;
using DAL.Services;

namespace MVC.Controllers;

public class VideoDetailController : Controller
{
    private readonly IVideoService _videoService;
    private readonly ICommentService _commentService;
    private readonly IPlaylistService _playlistService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<VideoDetailController> _logger;

    public VideoDetailController(
        IVideoService videoService,
        ICommentService commentService,
        IPlaylistService playlistService,
        ISubscriptionService subscriptionService,
        ICurrentUserService currentUserService,
        ILogger<VideoDetailController> logger)
    {
        _videoService = videoService;
        _commentService = commentService;
        _playlistService = playlistService;
        _subscriptionService = subscriptionService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IActionResult> Detail(int id, string? returnUrl = null)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
            return NotFound();

        var userId = _currentUserService.GetUserId();

        if (video.Creator.PricePerMonth.HasValue && video.Creator.PricePerMonth > 0)
        {
            bool isCreator = userId == video.Creator.Id;
            bool isSubscribed = false;

            if (userId != null && !isCreator)
            {
                var subscriptionResult = await _subscriptionService.IsUserSubscribedAsync(userId, video.Creator.Id);
                if (!subscriptionResult.IsError)
                {
                    isSubscribed = subscriptionResult.Value;
                }
            }

            if (!isCreator && !isSubscribed)
            {
                TempData["ErrorMessage"] = "You need to subscribe to watch this video.";
                return RedirectToAction("Index", "User", new { id = video.Creator.Id });
            }
        }

        var comments = await _commentService.GetByVideoIdAsync(id);
        
        var viewModel = new VideoDetailViewModel
        {
            Video = video,
            Comments = comments,
            ReturnUrl = returnUrl
        };

        if (!string.IsNullOrEmpty(userId))
        {
            var allPlaylists = await _playlistService.GetAllAsync();
            viewModel.UserPlaylists = allPlaylists.Where(p => p.Creator.Id == userId).ToList();

            foreach (var playlist in viewModel.UserPlaylists)
            {
                var playlistWithVideos = await _playlistService.GetByIdWithVideosAsync(playlist.Id);
                if (playlistWithVideos?.Item2.Any(v => v.Id == id) == true)
                {
                    viewModel.PlaylistsContainingVideo.Add(playlist.Id);
                }
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddToPlaylist(int videoId, int playlistId, string? returnUrl = null)
    {
        var userId = _currentUserService.GetUserId();

        var playlist = await _playlistService.GetByIdAsync(playlistId);
        if (playlist == null)
        {
            TempData["ErrorMessage"] = "Playlist not found.";
            return RedirectToAction(nameof(Detail), new { id = videoId, returnUrl });
        }

        if (playlist.Creator.Id != userId)
        {
            TempData["ErrorMessage"] = "You can only add videos to your own playlists.";
            return RedirectToAction(nameof(Detail), new { id = videoId, returnUrl });
        }

        var result = await _playlistService.AddVideoAsync(playlistId, videoId);
        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to add video to playlist.";
            _logger.LogError("Failed to add video {VideoId} to playlist {PlaylistId}: {Errors}",
                videoId, playlistId, result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = $"Video added to '{playlist.Name}' successfully!";
            _logger.LogInformation("Video {VideoId} added to playlist {PlaylistId} by user {UserId}",
                videoId, playlistId, userId);
        }

        return RedirectToAction(nameof(Detail), new { id = videoId, returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(int id, string content, int? parentCommentId = null, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["ErrorMessage"] = "Comment cannot be empty.";
            return RedirectToAction(nameof(Detail), new { id, returnUrl });
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Forbid();
        }

        var commentDto = new CommentCreateDto
        {
            VideoId = id,
            AuthorId = currentUserId,
            Content = content,
            ParentCommentId = parentCommentId
        };

        var result = await _commentService.CreateAsync(commentDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            TempData["ErrorMessage"] = "Failed to post comment. Please try again.";
            _logger.LogWarning("Failed to create comment for video {VideoId}: {Errors}", id, string.Join(", ", result.Errors));
        }
        else
        {
            TempData["SuccessMessage"] = parentCommentId.HasValue 
                ? "Reply posted successfully!" 
                : "Comment posted successfully!";
            _logger.LogInformation("Comment created for video {VideoId} by user {UserId}", id, currentUserId);
        }

        return RedirectToAction(nameof(Detail), new { id, returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id, int commentId, string? returnUrl = null)
    {
        var comment = await _commentService.GetByIdAsync(commentId);
        if (comment == null)
        {
            TempData["ErrorMessage"] = "Comment not found.";
            return RedirectToAction(nameof(Detail), new { id, returnUrl });
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId) || comment.Author.Id != currentUserId)
        {
            return Forbid();
        }

        var result = await _commentService.DeleteAsync(commentId);

        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to delete comment.";
            _logger.LogWarning("Failed to delete comment {CommentId}: {Errors}", commentId, string.Join(", ", result.Errors));
        }
        else
        {
            TempData["SuccessMessage"] = "Comment deleted successfully!";
            _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, currentUserId);
        }

        return RedirectToAction(nameof(Detail), new { id, returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> UploadThumbnail(int id, IFormFile thumbnailImage)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId) || video.Creator.Id != currentUserId)
        {
            return Forbid();
        }

        if (thumbnailImage == null || thumbnailImage.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select an image file.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        byte[]? thumbnailImageBytes = null;
        string? thumbnailImageFileName = null;

        await using var memoryStream = new MemoryStream();
        await thumbnailImage.CopyToAsync(memoryStream);
        thumbnailImageBytes = memoryStream.ToArray();
        thumbnailImageFileName = thumbnailImage.FileName;

        var videoUpdateDto = new VideoUpdateDto
        {
            Id = id,
            ThumbnailImageBytes = thumbnailImageBytes,
            ThumbnailImageFileName = thumbnailImageFileName
        };

        var result = await _videoService.UpdateAsync(videoUpdateDto);
        
        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            TempData["ErrorMessage"] = "Failed to upload thumbnail. Please try again.";
            _logger.LogWarning("Failed to upload thumbnail for video {VideoId}: {Errors}", id, string.Join(", ", result.Errors));
        }
        else
        {
            TempData["SuccessMessage"] = "Thumbnail uploaded successfully!";
            _logger.LogInformation("Thumbnail uploaded successfully for video {VideoId} by user {UserId}", id, currentUserId);
        }
        
        return RedirectToAction(nameof(Detail), new { id });
    }
}
