using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.DTOs;
using Infra.Repository;

namespace Business.Services;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<VideoCreateDto> _createValidator;
    private readonly IValidator<VideoUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly VideoMapper _mapper = new();

    public VideoService(
        IVideoRepository videoRepository, 
        IUserRepository userRepository,
        AppDbContext dbContext,
        IValidator<VideoCreateDto> createValidator,
        IValidator<VideoUpdateDto> updateValidator)
    {
        _videoRepository = videoRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<VideoDto>> GetAllAsync()
    {
        var videos = await _videoRepository.GetAllAsync();
        return _mapper.Map(videos);
    }

    public async Task<VideoDto?> GetByIdAsync(int id)
    {
        var video = await _videoRepository.GetByIdAsync(id);
        return video == null ? null : _mapper.Map(video);
    }

    public async Task<ErrorOr<VideoDto>> CreateAsync(VideoCreateDto dto)
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

        var video = new Video
        {
            Id = default,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Title = dto.Title,
            Description = dto.Description,
            Url = dto.Url,
            ThumbnailUrl = dto.ThumbnailUrl,
            CreatedAt = default,
            UpdatedAt = default
        };

        await _videoRepository.CreateAsync(video);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(video);
    }

    public async Task<ErrorOr<VideoDto>> UpdateAsync(VideoUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var video = await _videoRepository.GetByIdAsync(dto.Id);

        if (video is null)
        {
            return Error.NotFound();
        }

        if (!string.IsNullOrWhiteSpace(dto.CreatorId))
        {
            var newCreator = await _userRepository.GetByIdAsync(dto.CreatorId);
            if (newCreator is null)
            {
                return Error.NotFound();
            }

            video.CreatorId = dto.CreatorId;
            video.Creator = newCreator;
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            video.Title = dto.Title;
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            video.Description = dto.Description;
        }

        if (!string.IsNullOrWhiteSpace(dto.Url))
        {
            video.Url = dto.Url;
        }

        if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
        {
            video.ThumbnailUrl = dto.ThumbnailUrl;
        }

        await _videoRepository.UpdateAsync(video);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(video);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int id)
    {
        var video = await _videoRepository.GetByIdAsync(id);

        if (video is null)
        {
            return Error.NotFound();
        }

        await _videoRepository.DeleteAsync(video);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<List<VideoDto>> GetByFilterAsync(VideoFilterDto dto)
    {
        var videos = await _videoRepository.GetByFilterAsync(dto);
        return _mapper.Map(videos);
    }

    public async Task<PagedResultDto<VideoDto>> GetByFilterPagedAsync(VideoFilterDto dto)
    {
        var videos = await _videoRepository.GetByFilterAsync(dto);
        var totalCount = await _videoRepository.GetFilteredCountAsync(dto);

        return new PagedResultDto<VideoDto>
        {
            Items = _mapper.Map(videos),
            TotalCount = totalCount,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize
        };
    }
}
