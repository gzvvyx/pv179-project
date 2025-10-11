using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Playlist : BaseEntity
    {

        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        [InverseProperty(nameof(User.Playlists))]
        public required User Creator { get; set; }

        [InverseProperty(nameof(Video.Playlists))]
        public ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}
