namespace Business.DTOs
{
    public class PlaylistCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public required string CreatorId { get; set; }
    }
}
