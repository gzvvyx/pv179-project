namespace API.DTOs;

public class GetPlaylistsPagedRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CreatorId { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
