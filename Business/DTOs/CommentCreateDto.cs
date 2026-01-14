namespace Business.DTOs
{
    public class CommentCreateDto
    {
        public required string AuthorId { get; set; }
        public string? Content { get; set; }
        public required int VideoId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
