namespace Business.DTOs
{
    public class PlaylistUpdateDto
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
