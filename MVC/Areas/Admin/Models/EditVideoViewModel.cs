using Business.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MVC.Areas.Admin.Models;

public class EditVideoViewModel
{
    public VideoDto Video { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    
    [StringLength(200)]
    public string? Title { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [StringLength(500)]
    [Url]
    public string? Url { get; set; }
    
    [StringLength(500)]
    [Url]
    public string? ThumbnailUrl { get; set; }
    
    public IFormFile? ThumbnailImage { get; set; }
}

