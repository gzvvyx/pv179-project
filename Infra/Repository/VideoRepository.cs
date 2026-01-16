using DAL.Data;
using DAL.Models;
using Infra.DTOs;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class VideoRepository : IVideoRepository
{
    private const string VideoByIdCacheKeyPrefix = "Video_";
    
    private readonly AppDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public VideoRepository(AppDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public Task<List<Video>> GetAllAsync()
    {
        return _dbContext.Videos
            .Include(v => v.Creator)
            .Include(v => v.VideoCategories)
                .ThenInclude(vc => vc.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Video?> GetByIdAsync(int id)
    {
        var cacheKey = $"{VideoByIdCacheKeyPrefix}{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _dbContext.Videos
                .Include(video => video.Creator)
                .Include(video => video.VideoCategories)
                    .ThenInclude(vc => vc.Category)
                .FirstOrDefaultAsync(video => video.Id == id);
        });
    }

    public async Task CreateAsync(Video video)
    {
        if (video.Creator is not null)
        {
            _dbContext.Attach(video.Creator);
        }

        await _dbContext.Videos.AddAsync(video);
    }

    public async Task UpdateAsync(Video video)
    {
        if (video.Creator is not null)
        {
            _dbContext.Attach(video.Creator);
        }

        _dbContext.Videos.Update(video);
        _cacheService.Remove($"{VideoByIdCacheKeyPrefix}{video.Id}");
    }

    public async Task DeleteAsync(Video video)
    {
        _dbContext.Videos.Remove(video);
        _cacheService.Remove($"{VideoByIdCacheKeyPrefix}{video.Id}");
    }

    public async Task<List<Video>> GetByFilterAsync(VideoFilterDto dto)
    {
        var query = _dbContext.Videos
            .Include(v => v.Creator)
            .Include(v => v.VideoCategories)
                .ThenInclude(vc => vc.Category)
            .AsNoTracking()
            .AsQueryable();

        query = ApplyFilters(query, dto);

        query = dto.SortBy?.ToLower() switch
        {
            "title" => dto.SortDescending
                ? query.OrderByDescending(v => v.Title)
                : query.OrderBy(v => v.Title),
            "updatedat" => dto.SortDescending
                ? query.OrderByDescending(v => v.UpdatedAt)
                : query.OrderBy(v => v.UpdatedAt),
            _ => dto.SortDescending
                ? query.OrderByDescending(v => v.CreatedAt)
                : query.OrderBy(v => v.CreatedAt)
        };

        query = query
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize);

        return await query.ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(VideoFilterDto dto)
    {
        var query = _dbContext.Videos.AsQueryable();

        query = ApplyFilters(query, dto);

        return await query.CountAsync();
    }

    private IQueryable<Video> ApplyFilters(IQueryable<Video> query, VideoFilterDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Title) || !string.IsNullOrEmpty(dto.Description))
        {
            var searchTerm = dto.Title ?? dto.Description;
            query = query.Where(video =>
                EF.Functions.ILike(video.Title, $"%{searchTerm}%") ||
                EF.Functions.ILike(video.Description, $"%{searchTerm}%")
            );
        }

        if (!string.IsNullOrEmpty(dto.CreatorId))
        {
            query = query.Where(video => video.CreatorId == dto.CreatorId);
        }

        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            // AND logic: video must have ALL selected categories
            foreach (var categoryId in dto.CategoryIds)
            {
                var catId = categoryId; // Capture variable for closure
                query = query.Where(video => video.VideoCategories.Any(vc => vc.CategoryId == catId));
            }
        }
        else if (dto.CategoryId.HasValue)
        {
            query = query.Where(video => video.VideoCategories.Any(vc => vc.CategoryId == dto.CategoryId.Value));
        }

        if (dto.FromDate.HasValue)
        {
            query = query.Where(v => v.CreatedAt >= dto.FromDate.Value);
        }

        if (dto.ToDate.HasValue)
        {
            query = query.Where(v => v.CreatedAt <= dto.ToDate.Value);
        }

        return query;
    }
}

