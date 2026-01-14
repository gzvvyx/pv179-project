namespace Infra.DTOs;

public class CommentFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Content { get; set; }
    public string? AuthorId { get; set; }
    public int? VideoId { get; set; }
    public int? ParentCommentId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}