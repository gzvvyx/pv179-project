using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class UserViewMapper
{
    [MapProperty(nameof(UserDetailsDto.Videos), nameof(UserDetailsViewModel.UploadedVideos))]
    [MapProperty(nameof(UserDetailsDto.Playlists), nameof(UserDetailsViewModel.CreatedPlaylists))]
    [MapProperty(nameof(UserDetailsDto.Comments), nameof(UserDetailsViewModel.Comments))]

    [MapProperty("Videos.Count", nameof(UserDetailsViewModel.VideoCount))]
    [MapProperty("Playlists.Count", nameof(UserDetailsViewModel.PlaylistCount))]
    [MapProperty("Comments.Count", nameof(UserDetailsViewModel.CommentCount))]
    public partial UserDetailsViewModel MapToViewModel(UserDetailsDto userDto);

    private partial VideoItemVM MapVideo(VideoDto video);
    private partial PlaylistItemVM MapPlaylist(PlaylistDto playlist);
    private partial CommentItemVM MapComment(CommentDto comment);
}
