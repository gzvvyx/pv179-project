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
public class PlaylistController : Controller
{
    private readonly IPlaylistService _playlistService;
    private readonly IEditPlaylistViewModelFactory _viewModelFactory;
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(
        IPlaylistService playlistService,
        IEditPlaylistViewModelFactory viewModelFactory,
        ILogger<PlaylistController> logger)
    {
        _playlistService = playlistService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    [Route("", Name = "AdminPlaylists")]
    public async Task<IActionResult> Index()
    {
        var playlists = await _playlistService.GetAllAsync();
        var viewModels = playlists.Select(p => new PlaylistIndexViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatorUserName = p.Creator.UserName,
            CreatorId = p.Creator.Id,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var playlist = await _playlistService.GetByIdAsync(id);
        if (playlist == null)
        {
            return NotFound();
        }

        var model = await _viewModelFactory.CreateAsync(playlist);
        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPlaylistViewModel model)
    {
        var playlist = await _playlistService.GetByIdAsync(id);
        if (playlist == null)
        {
            return NotFound();
        }

        var playlistUpdateDto = new PlaylistUpdateDto
        {
            Id = id,
            Name = model.Name,
            Description = model.Description
        };

        var result = await _playlistService.UpdateAsync(playlistUpdateDto);
        
        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            
            await _viewModelFactory.PopulateViewModelAsync(model, playlist);
            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
}

