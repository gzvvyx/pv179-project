namespace Business.DTOs;

public class VideoCategoryDto
{
    public required int Id { get; set; }
    public required int VideoId { get; set; }
    public required int CategoryId { get; set; }
    public required bool IsPrimary { get; set; }
    public required CategoryDto Category { get; set; }
}
