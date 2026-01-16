using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class VideoCreateDto
{
    [Required]
    public required string CreatorId { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    [Url]
    public required string Url { get; set; }

    [Required]
    [Url]
    public required string ThumbnailUrl { get; set; }

    public List<int> CategoryIds { get; set; } = new();
    public int? PrimaryCategoryId { get; set; }
}

