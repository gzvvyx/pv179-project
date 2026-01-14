using Business.DTOs;
using ErrorOr;
using Infra.DTOs;

namespace Business.Services
{
    public interface IPlaylistService
    {
        Task<List<PlaylistDto>> GetAllAsync();
        Task<PlaylistDto?> GetByIdAsync(int id);
        Task<(PlaylistDto Playlist, List<VideoDto> Videos)?> GetByIdWithVideosAsync(int id);
        Task<ErrorOr<PlaylistDto>> CreateAsync(PlaylistCreateDto dto);
        Task<ErrorOr<PlaylistDto>> UpdateAsync(PlaylistUpdateDto dto);
        Task<ErrorOr<Success>> DeleteAsync(int id);
        Task<List<PlaylistDto>> GetByFilterAsync(PlaylistFilterDto dto);
        Task<PagedResultDto<PlaylistDto>> GetByFilterPagedAsync(PlaylistFilterDto dto);
        Task<ErrorOr<Success>> RemoveVideoAsync(int playlistId, int videoId);
        Task<ErrorOr<Success>> AddVideoAsync(int playlistId, int videoId);
    }
}
