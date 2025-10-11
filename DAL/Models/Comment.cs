using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{

    public class Comment : BaseEntity
    {
        [ForeignKey(nameof(Author))]
        public required string AuthorId { get; set; }

        [StringLength(500)]
        public required string Content { get; set; }

        [ForeignKey(nameof(Video))]
        public required int VideoId { get; set; }

        public int? ParentCommentId { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public Comment? ParentComment { get; set; }

        [InverseProperty(nameof(ParentComment))]
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        [InverseProperty(nameof(User.Comments))]
        public required User Author { get; set; }
        
        [InverseProperty(nameof(Video.Comments))]
        public required Video Video { get; set; }


    }
}
