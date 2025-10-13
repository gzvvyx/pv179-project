
using DAL.Models;

namespace Business.DTOs
{
    public class CommentDto
    {
        public required int Id { get; set; }
        public required string AuthorId { get; set; }

        public required string Content { get; set; }

        public required int VideoId { get; set; }

        public int ParentCommentId { get; set; }

        public Comment? ParentComment { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

    }
}
