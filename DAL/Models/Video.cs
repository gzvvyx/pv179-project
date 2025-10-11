using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Video : BaseEntity
    {
        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string ThumbnailUrl { get; set; }

        [InverseProperty(nameof(User.Videos))]
        public required User Creator { get; set; }

        [InverseProperty(nameof(Comment.Video))]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty(nameof(Playlist.Videos))]
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
