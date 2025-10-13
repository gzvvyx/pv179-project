namespace Business.DTOs
{
    public class PlaylistCreateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string CreatorId { get; set; }
    }
}
