namespace Business.DTOs;

public class VideoUpdateDto
{
    public required int Id { get; set; }
    public string? CreatorId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ThumbnailUrl { get; set; }
}

