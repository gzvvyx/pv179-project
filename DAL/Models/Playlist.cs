using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Playlist : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; set; }

    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    [InverseProperty(nameof(User.Playlists))]
    public required User Creator { get; set; }

    [InverseProperty(nameof(Video.Playlists))]
    public ICollection<Video> Videos { get; set; } = new List<Video>();
}
