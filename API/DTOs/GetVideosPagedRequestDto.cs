namespace API.DTOs;

public class GetVideosPagedRequestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public int? CategoryId { get; set; }
    public List<int>? CategoryIds { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
