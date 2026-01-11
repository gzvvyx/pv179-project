namespace Infra.DTOs;

public class VideoFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CreatorId { get; set; }
    
    public int? CategoryId { get; set; }
    public List<int>? CategoryIds { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
