using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MVC.Models;

public class VideoUploadViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, ErrorMessage = "Title cannot be longer than 255 characters")]
    [Display(Name = "Video Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description cannot be longer than 2000 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Video File")]
    public IFormFile? VideoFile { get; set; }

    [Display(Name = "Thumbnail Image")]
    public IFormFile? ThumbnailFile { get; set; }

    [Display(Name = "Video URL")]
    public string? VideoUrl { get; set; }

    [Display(Name = "Thumbnail URL")]
    public string? ThumbnailUrl { get; set; }

    [Display(Name = "Categories")]
    public List<int> SelectedCategoryIds { get; set; } = new();

    [Display(Name = "Primary Category")]
    public int? PrimaryCategoryId { get; set; }
}
