using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
    public class CommentCreateDto
    {
        [Required]
        public required string AuthorId { get; set; }
        [Required]
        public required string Content { get; set; }
        [Required]
        public required int VideoId { get; set; }
    }
}
