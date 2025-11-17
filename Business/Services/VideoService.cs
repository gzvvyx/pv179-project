using Business.DTOs;
using Infra.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUserRepository _userRepository;
    private readonly VideoMapper _mapper = new();

    public VideoService(IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _videoRepository = videoRepository;
        _userRepository = userRepository;
    }

    public async Task<List<VideoDto>> GetAllAsync()
    {
        var videos = await _videoRepository.GetAllAsync();
        return _mapper.Map(videos);
    }

    public async Task<VideoDto?> GetByIdAsync(int id)
    {
        var video = await _videoRepository.GetByIdAsync(id);
        return video is null ? null : _mapper.Map(video);
    }

    public async Task<(IdentityResult Result, VideoDto? Video)> CreateAsync(VideoCreateDto dto)
    {
        var creator = await _userRepository.GetUserByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "CreatorNotFound",
                Description = $"Creator with id '{dto.CreatorId}' was not found."
            }), null);
        }

        var timestamp = DateTime.UtcNow;

        var video = new Video
        {
            Id = default,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Title = dto.Title,
            Description = dto.Description,
            Url = dto.Url,
            ThumbnailUrl = dto.ThumbnailUrl,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };

        await _videoRepository.AddAsync(video);

        return (IdentityResult.Success, _mapper.Map(video));
    }

    public async Task<(IdentityResult Result, VideoDto? Video)> UpdateAsync(int id, VideoUpdateDto dto)
    {
        var video = await _videoRepository.GetByIdAsync(id);

        if (video is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "VideoNotFound",
                Description = $"Video with id '{id}' was not found."
            }), null);
        }

        if (!string.IsNullOrWhiteSpace(dto.CreatorId) && !string.Equals(video.CreatorId, dto.CreatorId, StringComparison.Ordinal))
        {
            var newCreator = await _userRepository.GetUserByIdAsync(dto.CreatorId);
            if (newCreator is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "CreatorNotFound",
                    Description = $"Creator with id '{dto.CreatorId}' was not found."
                }), null);
            }

            video.CreatorId = dto.CreatorId;
            video.Creator = newCreator;
        }

        if (!string.IsNullOrWhiteSpace(dto.Title) && !string.Equals(video.Title, dto.Title, StringComparison.Ordinal))
        {
            video.Title = dto.Title;
        }

        if (!string.IsNullOrWhiteSpace(dto.Description) && !string.Equals(video.Description, dto.Description, StringComparison.Ordinal))
        {
            video.Description = dto.Description;
        }

        if (!string.IsNullOrWhiteSpace(dto.Url) && !string.Equals(video.Url, dto.Url, StringComparison.OrdinalIgnoreCase))
        {
            video.Url = dto.Url;
        }

        if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl) && !string.Equals(video.ThumbnailUrl, dto.ThumbnailUrl, StringComparison.OrdinalIgnoreCase))
        {
            video.ThumbnailUrl = dto.ThumbnailUrl;
        }

        video.UpdatedAt = DateTime.UtcNow;

        await _videoRepository.UpdateAsync(video);

        return (IdentityResult.Success, _mapper.Map(video));
    }

    public async Task<IdentityResult> DeleteAsync(int id)
    {
        var video = await _videoRepository.GetByIdAsync(id);

        if (video is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "VideoNotFound",
                Description = $"Video with id '{id}' was not found."
            });
        }

        await _videoRepository.DeleteAsync(video);

        return IdentityResult.Success;
    }

    public async Task<List<VideoDto>> GetByFilterAsync(VideoFilterDto dto)
    {
        var videos = await _videoRepository.GetByFilterAsync(dto);

        return _mapper.Map(videos);
    }
}
