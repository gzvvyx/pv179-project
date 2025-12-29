namespace Business.DTOs;

public class UserDetailsDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }

    public List<VideoDto> Videos { get; set; } = [];
    public List<PlaylistDto> Playlists { get; set; } = [];
    public List<CommentDto> Comments { get; set; } = [];
}
