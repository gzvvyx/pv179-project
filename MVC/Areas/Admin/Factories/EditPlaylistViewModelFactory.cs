using Business.DTOs;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditPlaylistViewModelFactory : IEditPlaylistViewModelFactory
{
    public Task<EditPlaylistViewModel> CreateAsync(PlaylistDto playlist)
    {
        var model = new EditPlaylistViewModel
        {
            Playlist = playlist,
            CreatorName = playlist.Creator.UserName,
            Name = playlist.Name,
            Description = playlist.Description
        };
        
        return Task.FromResult(model);
    }

    public Task PopulateViewModelAsync(EditPlaylistViewModel model, PlaylistDto playlist)
    {
        model.Playlist = playlist;
        model.CreatorName = playlist.Creator.UserName;
        
        return Task.CompletedTask;
    }
}

