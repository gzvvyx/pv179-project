using Business.DTOs;

namespace pv179.Models;

public class AddVideosToPlaylistViewModel
{
    public required PlaylistDto Playlist { get; set; }
    public required List<VideoDto> UserVideos { get; set; }
    public required List<int> VideosInPlaylist { get; set; }
}
