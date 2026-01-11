using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Services;
using Business.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;
using MVC.Extensions;

namespace MVC.Controllers;

public class VideoDetailController : Controller
{
    private readonly IVideoService _videoService;
    private readonly ILogger<VideoDetailController> _logger;

    public VideoDetailController(IVideoService videoService, ILogger<VideoDetailController> logger)
    {
        _videoService = videoService;
        _logger = logger;
    }

    public async Task<IActionResult> Detail(int id)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
            return NotFound();
        return View(video);
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
