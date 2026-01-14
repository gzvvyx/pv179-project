using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Services;
using Business.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;
using MVC.Extensions;
using MVC.Models;

namespace MVC.Controllers;

public class VideoDetailController : Controller
{
    private readonly IVideoService _videoService;
    private readonly ICommentService _commentService;
    private readonly ILogger<VideoDetailController> _logger;

    public VideoDetailController(
        IVideoService videoService,
        ICommentService commentService,
        ILogger<VideoDetailController> logger)
    {
        _videoService = videoService;
        _commentService = commentService;
        _logger = logger;
    }

    public async Task<IActionResult> Detail(int id, string? returnUrl = null)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
            return NotFound();

        var comments = await _commentService.GetByVideoIdAsync(id);
        
        var viewModel = new VideoDetailViewModel
        {
            Video = video,
            Comments = comments,
            ReturnUrl = returnUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(int id, string content, string? returnUrl = null)
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
            Content = content
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
