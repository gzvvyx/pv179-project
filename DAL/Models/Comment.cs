using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{

    public class Comment
    {
        public required int Id { get; set; }

        [ForeignKey(nameof(Author))]
        public required string AuthorId { get; set; }

        [StringLength(500)]
        public required string Content { get; set; }

        public required DateTime CommentDate { get; set; }

        [ForeignKey(nameof(Video))]
        public required int VideoId { get; set; }

        public required int ParentCommentId { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public required Comment ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public required User Author { get; set; }
        public required Video Video { get; set; }


    }
}
