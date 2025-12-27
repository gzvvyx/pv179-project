using Business.DTOs;
using Infra.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public interface IPlaylistService
    {
        Task<List<PlaylistDto>> GetAllAsync();
        Task<PlaylistDto?> GetByIdAsync(int id);
        Task<(IdentityResult Result, PlaylistDto? Playlist)> CreateAsync(PlaylistCreateDto dto);
        Task<(IdentityResult Result, PlaylistDto? Playlist)> UpdateAsync(int id, PlaylistUpdateDto dto);
        Task<IdentityResult> DeleteAsync(int id);
        Task<List<PlaylistDto>> GetByFilterAsync(PlaylistFilterDto dto);
        Task<PagedResultDto<PlaylistDto>> GetByFilterPagedAsync(PlaylistFilterDto dto);
    }
}
