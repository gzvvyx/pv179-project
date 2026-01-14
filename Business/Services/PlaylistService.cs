using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.DTOs;
using Infra.Repository;

namespace Business.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<PlaylistCreateDto> _createValidator;
        private readonly IValidator<PlaylistUpdateDto> _updateValidator;
        private readonly AppDbContext _dbContext;
        private readonly PlaylistMapper _playlistMapper = new();
        private readonly VideoMapper _videoMapper = new();

        public PlaylistService(
            IPlaylistRepository playlistRepository, 
            IVideoRepository videoRepository, 
            IUserRepository userRepository,
            AppDbContext dbContext,
            IValidator<PlaylistCreateDto> createValidator,
            IValidator<PlaylistUpdateDto> updateValidator)
        {
            _playlistRepository = playlistRepository;
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<PlaylistDto>> GetAllAsync()
        {
            var playlists = await _playlistRepository.GetAllAsync();
            return _playlistMapper.Map(playlists);
        }

        public async Task<PlaylistDto?> GetByIdAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            return playlist == null ? null : _playlistMapper.Map(playlist);
        }

        public async Task<(PlaylistDto Playlist, List<VideoDto> Videos)?> GetByIdWithVideosAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdWithVideosAsync(id);
            if (playlist == null)
            {
                return null;
            }

            var playlistDto = _playlistMapper.Map(playlist);
            var videoDtos = _videoMapper.Map(playlist.Videos.ToList());

            return (playlistDto, videoDtos);
        }

        public async Task<ErrorOr<PlaylistDto>> CreateAsync(PlaylistCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return validationResult.ToErrors();
            }

            var creator = await _userRepository.GetByIdAsync(dto.CreatorId);
            if (creator is null)
            {
                return Error.NotFound();
            }

            var playlist = new Playlist
            {
                Id = default,
                Name = dto.Name!,
                Description = dto.Description,
                CreatorId = dto.CreatorId,
                Creator = creator,
                Videos = new List<Video>(),
                CreatedAt = default,
                UpdatedAt = default
            };

            await _playlistRepository.CreateAsync(playlist);
            await _dbContext.SaveChangesAsync();

            return _playlistMapper.Map(playlist);
        }

        public async Task<ErrorOr<PlaylistDto>> UpdateAsync(PlaylistUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return validationResult.ToErrors();
            }

            var playlist = await _playlistRepository.GetByIdAsync(dto.Id);
            if (playlist is null)
            {
                return Error.NotFound();
            }

            if (dto.Name != null)
            {
                playlist.Name = dto.Name;
            }

            playlist.Description = dto.Description;

            await _playlistRepository.UpdateAsync(playlist);
            await _dbContext.SaveChangesAsync();

            return _playlistMapper.Map(playlist);
        }

        public async Task<ErrorOr<Success>> DeleteAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist is null)
            {
                return Error.NotFound();
            }

            await _playlistRepository.DeleteAsync(playlist);
            await _dbContext.SaveChangesAsync();

            return Result.Success;
        }

        public async Task<ErrorOr<Success>> RemoveVideoAsync(int playlistId, int videoId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist is null)
            {
                return Error.NotFound("Playlist not found");
            }

            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video is null)
            {
                return Error.NotFound("Video not found");
            }

            await _playlistRepository.RemoveVideoFromPlaylistAsync(playlist, video);
            return Result.Success;
        }

        public async Task<ErrorOr<Success>> AddVideoAsync(int playlistId, int videoId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist is null)
            {
                return Error.NotFound("Playlist not found");
            }

            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video is null)
            {
                return Error.NotFound("Video not found");
            }

            await _playlistRepository.AddVideoToPlaylistAsync(playlist, video);
            return Result.Success;
        }

        public async Task<List<PlaylistDto>> GetByFilterAsync(PlaylistFilterDto dto)
        {
            var playlists = await _playlistRepository.GetByFilterAsync(dto);
            return _playlistMapper.Map(playlists);
        }

        public async Task<PagedResultDto<PlaylistDto>> GetByFilterPagedAsync(PlaylistFilterDto dto)
        {
            var playlists = await _playlistRepository.GetByFilterAsync(dto);
            var totalCount = await _playlistRepository.GetFilteredCountAsync(dto);

            return new PagedResultDto<PlaylistDto>
            {
                Items = _playlistMapper.Map(playlists),
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }
    }
}
