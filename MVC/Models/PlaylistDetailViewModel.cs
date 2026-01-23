using Business.DTOs;

namespace pv179.Models;

public class PlaylistDetailViewModel
{
    public required PlaylistDto Playlist { get; set; }
    public required List<VideoDto> Videos { get; set; }
    public required bool IsOwner { get; set; }
}