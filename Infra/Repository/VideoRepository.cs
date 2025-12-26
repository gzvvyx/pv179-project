using DAL.Data;
using DAL.Models;
using Infra.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class VideoRepository : IVideoRepository
{
    private readonly AppDbContext _dbContext;

    public VideoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Video>> GetAllAsync()
    {
        return _dbContext.Videos
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Video?> GetByIdAsync(int id)
    {
        return _dbContext.Videos
            .Include(video => video.Creator)
            .FirstOrDefaultAsync(video => video.Id == id);
    }

    public async Task AddAsync(Video video)
    {
        if (video.Creator is not null)
        {
            _dbContext.Attach(video.Creator);
        }

        await _dbContext.Videos.AddAsync(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Video video)
    {
        if (video.Creator is not null)
        {
            _dbContext.Attach(video.Creator);
        }

        _dbContext.Videos.Update(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Video video)
    {
        _dbContext.Videos.Remove(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Video>> GetByFilterAsync(VideoFilterDto dto)
    {
        var query = _dbContext.Videos.AsQueryable();
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

        return await query.ToListAsync();
    }
}

