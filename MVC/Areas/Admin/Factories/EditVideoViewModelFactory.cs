using Business.DTOs;
using Business.Services;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditVideoViewModelFactory : IEditVideoViewModelFactory
{
    private readonly ICategoryService _categoryService;

    public EditVideoViewModelFactory(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<EditVideoViewModel> CreateAsync(VideoDto video)
    {
        var categories = await _categoryService.GetAllAsync();
        var selectedCategoryIds = video.Categories.Select(c => c.CategoryId).ToList();
        var primaryCategory = video.Categories.FirstOrDefault(c => c.IsPrimary);
        
        var model = new EditVideoViewModel
        {
            Video = video,
            CreatorName = video.Creator.UserName,
            Title = video.Title,
            Description = video.Description,
            Url = video.Url,
            ThumbnailUrl = video.ThumbnailUrl,
            AvailableCategories = categories,
            SelectedCategoryIds = selectedCategoryIds,
            PrimaryCategoryId = primaryCategory?.CategoryId
        };
        
        return model;
    }

    public async Task PopulateViewModelAsync(EditVideoViewModel model, VideoDto video)
    {
        model.Video = video;
        model.CreatorName = video.Creator.UserName;
        model.AvailableCategories = await _categoryService.GetAllAsync();
        model.SelectedCategoryIds = video.Categories.Select(c => c.CategoryId).ToList();
        model.PrimaryCategoryId = video.Categories.FirstOrDefault(c => c.IsPrimary)?.CategoryId;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _categoryService.GetAllAsync();
    }
}

