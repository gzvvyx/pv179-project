using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.DTOs;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly PlaylistMapper _mapper = new();

        public PlaylistService(IPlaylistRepository playlistRepository, IVideoRepository videoRepository, IUserRepository userRepository)
        {
            _playlistRepository = playlistRepository;
            _videoRepository = videoRepository;
            _userRepository = userRepository;
        }

        public async Task<List<PlaylistDto>> GetAllAsync()
        {
            var playlists = await _playlistRepository.GetAllAsync();
            return _mapper.Map(playlists);
        }

        public async Task<PlaylistDto?> GetByIdAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            return playlist is null ? null : _mapper.Map(playlist);
        }

        public async Task<(IdentityResult Result, PlaylistDto? Playlist)> CreateAsync(PlaylistCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.CreatorId);
            if (user is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"User with id '{dto.CreatorId}' was not found."
                }), null);
            }
            var timestamp = DateTime.UtcNow;
            var playlist = new Playlist()
            {
                Id = default,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = timestamp,
                UpdatedAt = timestamp,
                CreatorId = dto.CreatorId,
                Creator = user,
                Videos = new List<Video>()
            };

            await _playlistRepository.CreateAsync(playlist);
            return (IdentityResult.Success, _mapper.Map(playlist));
        }

        public async Task<(IdentityResult Result, PlaylistDto? Playlist)> UpdateAsync(int id, PlaylistUpdateDto dto)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "PlaylistNotFound",
                    Description = $"Playlist with id '{id}' was not found."
                }), null);
            }

            playlist.Name = dto.Name;
            playlist.Description = dto.Description;
            playlist.UpdatedAt = DateTime.UtcNow;

            await _playlistRepository.UpdateAsync(playlist);
            return (IdentityResult.Success, _mapper.Map(playlist));
        }

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist is null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "PlaylistNotFound",
                    Description = $"Playlist with id '{id}' was not found."
                });
            }

            await _playlistRepository.DeleteAsync(playlist);
            return IdentityResult.Success;
        }

        public async Task<List<PlaylistDto>> GetByFilterAsync(PlaylistFilterDto dto)
        {
            var playlists = await _playlistRepository.GetByFilterAsync(dto);
            return _mapper.Map(playlists);
        }

        public async Task<PagedResultDto<PlaylistDto>> GetByFilterPagedAsync(PlaylistFilterDto dto)
        {
            var playlists = await _playlistRepository.GetByFilterAsync(dto);
            var totalCount = await _playlistRepository.GetFilteredCountAsync(dto);

            return new PagedResultDto<PlaylistDto>
            {
                Items = _mapper.Map(playlists),
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }
    }
}
