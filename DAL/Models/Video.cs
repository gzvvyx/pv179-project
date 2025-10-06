using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Video
    {
        public required int Id { get; set; }
        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string ThumbnailUrl { get; set; }
        public required DateTime UploadDate { get; set; }

        public required User Creator { get; set; }
    }
}