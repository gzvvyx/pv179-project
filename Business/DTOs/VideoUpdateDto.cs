using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class VideoUpdateDto
{
    public string? CreatorId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    [Url]
    public string? Url { get; set; }

    [Url]
    public string? ThumbnailUrl { get; set; }
}

