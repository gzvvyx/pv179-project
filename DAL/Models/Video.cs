using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Video : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required string Url { get; set; }

    [Required]
    public required string ThumbnailUrl { get; set; }

    [Required]
    [InverseProperty(nameof(User.Videos))]
    public required User Creator { get; set; }

    [InverseProperty(nameof(Comment.Video))]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty(nameof(Playlist.Videos))]
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    [InverseProperty(nameof(VideoCategory.Video))]
    public ICollection<VideoCategory> VideoCategories { get; set; } = new List<VideoCategory>();
}
