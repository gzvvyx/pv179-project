using Business.DTOs;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public interface IEditVideoViewModelFactory
{
    Task<EditVideoViewModel> CreateAsync(VideoDto video);
    Task PopulateViewModelAsync(EditVideoViewModel model, VideoDto video);
}

