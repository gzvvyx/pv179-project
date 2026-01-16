using Business.DTOs;
using Business.Services;
using DAL.Services;
using Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers;

[Authorize]
public class VideoController : Controller
{
    private readonly IVideoService _videoService;
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly ILogger<VideoController> _logger;

    public VideoController(
        IVideoService videoService,
        ICategoryService categoryService,
        ICurrentUserService currentUserService,
        IFileService fileService,
        ILogger<VideoController> logger)
    {
        _videoService = videoService;
        _categoryService = categoryService;
        _currentUserService = currentUserService;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;
        
        var model = new VideoUploadViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VideoUploadViewModel model)
    {
        if (model.VideoFile == null)
        {
            ModelState.AddModelError(nameof(model.VideoFile), "Video file is required");
        }

        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            return View(model);
        }

        var userId = _currentUserService.GetUserId();

        try
        {
            string videoUrl = null;
            if (model.VideoFile != null)
            {
                using var videoStream = model.VideoFile.OpenReadStream();
                var relativePath = await _fileService.SaveFileAsync(videoStream, model.VideoFile.FileName, "videos");
                videoUrl = $"/uploads/{relativePath}";
            }

            string thumbnailUrl = "/images/default-thumbnail.jpg";
            if (model.ThumbnailFile != null)
            {
                using var thumbnailStream = model.ThumbnailFile.OpenReadStream();
                var relativePath = await _fileService.SaveFileAsync(thumbnailStream, model.ThumbnailFile.FileName, "thumbnails");
                thumbnailUrl = $"/uploads/{relativePath}";
            }

            var createDto = new VideoCreateDto
            {
                CreatorId = userId,
                Title = model.Title,
                Description = model.Description,
                Url = videoUrl,
                ThumbnailUrl = thumbnailUrl,
                CategoryIds = model.SelectedCategoryIds,
                PrimaryCategoryId = model.PrimaryCategoryId
            };

            var result = await _videoService.CreateAsync(createDto);

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories;
                return View(model);
            }

            var video = result.Value;

            _logger.LogInformation("Video {VideoId} uploaded successfully by user {UserId}", video.Id, userId);
            TempData["SuccessMessage"] = "Video uploaded successfully!";

            return RedirectToAction("Detail", "VideoDetail", new { id = video.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading video for user {UserId}", userId);
            ModelState.AddModelError(string.Empty, "An error occurred while uploading the video. Please try again.");
            
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _currentUserService.GetUserId();

        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        if (video.Creator.Id != userId)
        {
            return Forbid();
        }

        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;

        var model = new VideoUploadViewModel
        {
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.Url,
            ThumbnailUrl = video.ThumbnailUrl,
            SelectedCategoryIds = video.Categories.Select(vc => vc.Category.Id).ToList(),
            PrimaryCategoryId = video.Categories.FirstOrDefault(vc => vc.IsPrimary)?.Category.Id
        };

        ViewBag.VideoId = id;
        ViewBag.CurrentThumbnail = video.ThumbnailUrl;
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VideoUploadViewModel model)
    {
        var userId = _currentUserService.GetUserId();


        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        if (video.Creator.Id != userId)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.VideoId = id;
            ViewBag.CurrentThumbnail = video.ThumbnailUrl;
            model.VideoUrl = video.Url;
            return View(model);
        }

        try
        {
            string videoUrl = null;
            if (model.VideoFile != null)
            {
                using var videoStream = model.VideoFile.OpenReadStream();
                var relativePath = await _fileService.SaveFileAsync(videoStream, model.VideoFile.FileName, "videos");
                videoUrl = $"/uploads/{relativePath}";
            }

            string thumbnailUrl = null;
            if (model.ThumbnailFile != null)
            {
                using var thumbnailStream = model.ThumbnailFile.OpenReadStream();
                var relativePath = await _fileService.SaveFileAsync(thumbnailStream, model.ThumbnailFile.FileName, "thumbnails");
                thumbnailUrl = $"/uploads/{relativePath}";
            }

            var updateDto = new VideoUpdateDto
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                Url = videoUrl,
                ThumbnailUrl = thumbnailUrl,
                CategoryIds = model.SelectedCategoryIds,
                PrimaryCategoryId = model.PrimaryCategoryId
            };

            var result = await _videoService.UpdateAsync(updateDto);

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories;
                ViewBag.VideoId = id;
                ViewBag.CurrentThumbnail = video.ThumbnailUrl;
                return View(model);
            }

            _logger.LogInformation("Video {VideoId} updated successfully by user {UserId}", id, userId);
            TempData["SuccessMessage"] = "Video updated successfully!";

            return RedirectToAction("Detail", "VideoDetail", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating video {VideoId} for user {UserId}", id, userId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the video. Please try again.");
            
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.VideoId = id;
            ViewBag.CurrentThumbnail = video.ThumbnailUrl;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _currentUserService.GetUserId();

        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        if (video.Creator.Id != userId)
        {
            return Forbid();
        }

        var result = await _videoService.DeleteAsync(id);

        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to delete video.";
            return RedirectToAction("Detail", "VideoDetail", new { id });
        }

        TempData["SuccessMessage"] = "Video deleted successfully.";
        return RedirectToAction("Index", "Home");
    }
}
