namespace Business.DTOs;

public class VideoDto
{
    public required int Id { get; set; }
    public required string CreatorId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
    public required string ThumbnailUrl { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}

