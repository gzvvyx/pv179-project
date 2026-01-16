using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.DTOs;
using Infra.Repository;
using Infra.Services;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;
    private readonly IValidator<VideoCreateDto> _createValidator;
    private readonly IValidator<VideoUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<VideoService> _logger;
    private readonly VideoMapper _mapper = new();

    public VideoService(
        IVideoRepository videoRepository, 
        IUserRepository userRepository,
        IFileService fileService,
        AppDbContext dbContext,
        IValidator<VideoCreateDto> createValidator,
        IValidator<VideoUpdateDto> updateValidator,
        ILogger<VideoService> logger)
    {
        _videoRepository = videoRepository;
        _userRepository = userRepository;
        _fileService = fileService;
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
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

        if (dto.ThumbnailImageBytes != null && dto.ThumbnailImageBytes.Length > 0 && 
            !string.IsNullOrWhiteSpace(dto.ThumbnailImageFileName))
        {
            var thumbnailResult = await ProcessThumbnailUploadAsync(
                dto.ThumbnailImageBytes, 
                dto.ThumbnailImageFileName, 
                video.ThumbnailUrl);
            
            if (thumbnailResult.IsError)
            {
                return thumbnailResult.FirstError;
            }
            
            video.ThumbnailUrl = thumbnailResult.Value;
        }
        else if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
        {
            video.ThumbnailUrl = dto.ThumbnailUrl;
        }

        // Update categories if provided
        var categoriesUpdated = false;
        if (dto.CategoryIds != null)
        {
            // Remove existing categories
            _dbContext.Set<VideoCategory>().RemoveRange(video.VideoCategories);
            
            // Add new categories
            var now = DateTime.UtcNow;
            var primaryId = dto.PrimaryCategoryId ?? dto.CategoryIds.FirstOrDefault();
            var newCategories = dto.CategoryIds.Select(categoryId => new VideoCategory
            {
                Id = default,
                VideoId = video.Id,
                CategoryId = categoryId,
                IsPrimary = categoryId == primaryId,
                CreatedAt = now,
                UpdatedAt = now
            }).ToList();
            
            video.VideoCategories = newCategories;
            categoriesUpdated = true;
        }

        await _videoRepository.UpdateAsync(video);
        await _dbContext.SaveChangesAsync();

        // Reload video to get navigation properties if categories were updated
        if (categoriesUpdated)
        {
            video = (await _videoRepository.GetByIdAsync(dto.Id))!;
        }

        return _mapper.Map(video);
    }

    private async Task<ErrorOr<string>> ProcessThumbnailUploadAsync(
        byte[] fileBytes, 
        string fileName, 
        string? currentThumbnailUrl = null)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Error.Validation(
                nameof(fileName),
                "Only image files (jpg, jpeg, png, gif, webp) are allowed.");
        }

        const long maxFileSize = 5 * 1024 * 1024;
        if (fileBytes.Length > maxFileSize)
        {
            return Error.Validation(
                nameof(fileBytes),
                "File size must be less than 5MB.");
        }

        try
        {
            if (!string.IsNullOrEmpty(currentThumbnailUrl) && 
                !currentThumbnailUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var oldThumbnailPath = currentThumbnailUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)
                    ? currentThumbnailUrl.Substring("/uploads/".Length)
                    : currentThumbnailUrl;
                
                await _fileService.DeleteFileAsync(oldThumbnailPath);
            }

            var relativePath = await _fileService.SaveFileAsync(fileBytes, fileName, "thumbnails");
            
            var thumbnailUrl = $"/uploads/{relativePath}";
            
            _logger.LogInformation("Thumbnail uploaded successfully: {ThumbnailUrl}", thumbnailUrl);
            
            return thumbnailUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading thumbnail image: {FileName}", fileName);
            return Error.Failure(
                nameof(fileBytes),
                "An error occurred while uploading the image. Please try again.");
        }
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
