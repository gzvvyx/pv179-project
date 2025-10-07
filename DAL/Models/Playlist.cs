using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Playlist
    {
        public required int Id { get; set; }

        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required DateTime CreatedAt { get; set; }

        public required User Creator { get; set; }
        public ICollection<Video> Videos { get; set; } = new List<Video>();


    }
}
