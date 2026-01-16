using Business.DTOs;
using Business.Services;
using DAL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Admin.Factories;
using MVC.Areas.Admin.Models;
using MVC.Extensions;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class VideoController : Controller
{
    private readonly IVideoService _videoService;
    private readonly IEditVideoViewModelFactory _viewModelFactory;
    private readonly ILogger<VideoController> _logger;

    public VideoController(
        IVideoService videoService,
        IEditVideoViewModelFactory viewModelFactory,
        ILogger<VideoController> logger)
    {
        _videoService = videoService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    [Route("", Name = "AdminVideos")]
    public async Task<IActionResult> Index()
    {
        var videos = await _videoService.GetAllAsync();
        var viewModels = videos.Select(v => new VideoIndexViewModel
        {
            Id = v.Id,
            Title = v.Title,
            Description = v.Description,
            Url = v.Url,
            CreatorUserName = v.Creator.UserName,
            CreatorId = v.Creator.Id,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        var model = await _viewModelFactory.CreateAsync(video);
        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditVideoViewModel model)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
        {
            return NotFound();
        }

        byte[]? thumbnailImageBytes = null;
        string? thumbnailImageFileName = null;

        // Read file bytes if a new image was provided
        if (model.ThumbnailImage != null && model.ThumbnailImage.Length > 0)
        {
            await using var memoryStream = new MemoryStream();
            await model.ThumbnailImage.CopyToAsync(memoryStream);
            thumbnailImageBytes = memoryStream.ToArray();
            thumbnailImageFileName = model.ThumbnailImage.FileName;
        }

        var videoUpdateDto = new VideoUpdateDto
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            Url = model.Url,
            ThumbnailUrl = model.ThumbnailUrl,
            ThumbnailImageBytes = thumbnailImageBytes,
            ThumbnailImageFileName = thumbnailImageFileName,
            CategoryIds = model.SelectedCategoryIds,
            PrimaryCategoryId = model.PrimaryCategoryId
        };

        var result = await _videoService.UpdateAsync(videoUpdateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);

            await _viewModelFactory.PopulateViewModelAsync(model, video);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
}

