namespace Business.DTOs
{
    public class PlaylistDto
    {
        public required int Id { get; set; }
        public required string CreatorId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
