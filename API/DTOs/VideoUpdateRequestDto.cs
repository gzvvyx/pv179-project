namespace API.DTOs;

public class VideoUpdateRequestDto
{
    public string? CreatorId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailImageBase64 { get; set; }
    public string? ThumbnailImageFileName { get; set; }
}

