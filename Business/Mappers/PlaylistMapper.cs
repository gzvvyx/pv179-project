using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers
{
    [Mapper]
    public partial class PlaylistMapper
    {
        public partial List<PlaylistDto> Map(List<Playlist> playlists);
        public partial PlaylistDto Map(Playlist playlist);
    }
}
