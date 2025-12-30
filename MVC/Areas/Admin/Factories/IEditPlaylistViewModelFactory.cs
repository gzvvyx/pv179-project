using Business.DTOs;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public interface IEditPlaylistViewModelFactory
{
    Task<EditPlaylistViewModel> CreateAsync(PlaylistDto playlist);
    Task PopulateViewModelAsync(EditPlaylistViewModel model, PlaylistDto playlist);
}

