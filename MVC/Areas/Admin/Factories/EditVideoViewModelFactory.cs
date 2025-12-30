using Business.DTOs;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditVideoViewModelFactory : IEditVideoViewModelFactory
{
    public Task<EditVideoViewModel> CreateAsync(VideoDto video)
    {
        var model = new EditVideoViewModel
        {
            Video = video,
            CreatorName = video.Creator.UserName,
            Title = video.Title,
            Description = video.Description,
            Url = video.Url,
            ThumbnailUrl = video.ThumbnailUrl
        };
        
        return Task.FromResult(model);
    }

    public Task PopulateViewModelAsync(EditVideoViewModel model, VideoDto video)
    {
        model.Video = video;
        model.CreatorName = video.Creator.UserName;
        
        return Task.CompletedTask;
    }
}

