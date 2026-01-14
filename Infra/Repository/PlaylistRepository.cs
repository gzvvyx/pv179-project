using DAL.Data;
using DAL.Models;
using Infra.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly AppDbContext _dbContext;

        public PlaylistRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Playlist>> GetAllAsync()
        {
            return _dbContext.Playlists
                .AsNoTracking()
                .Include(playlist => playlist.Creator)
                .ToListAsync();
        }

        public Task<Playlist?> GetByIdAsync(int id)
        {
            return _dbContext.Playlists
                .Include(playlist => playlist.Creator)
                .Include(playlist => playlist.Videos)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public Task<Playlist?> GetByIdWithVideosAsync(int id)
        {
            return _dbContext.Playlists
                .AsNoTracking()
                .Include(playlist => playlist.Creator)
                .Include(playlist => playlist.Videos)
                    .ThenInclude(video => video.Creator)
                .Include(playlist => playlist.Videos)
                    .ThenInclude(video => video.VideoCategories)
                        .ThenInclude(vc => vc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(Playlist playlist)
        {
            if (playlist.Creator is not null)
            {
                _dbContext.Attach(playlist.Creator);
            }

            await _dbContext.Playlists.AddAsync(playlist);
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            if (playlist.Creator is not null)
            {
                _dbContext.Attach(playlist.Creator);
            }

            _dbContext.Playlists.Update(playlist);
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _dbContext.Playlists.Remove(playlist);
        }

        public async Task RemoveVideoFromPlaylistAsync(Playlist playlist, Video video)
        {
            if (playlist.Videos.Contains(video))
            {
                playlist.Videos.Remove(video);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddVideoToPlaylistAsync(Playlist playlist, Video video)
        {
            if (!playlist.Videos.Contains(video))
            {
                playlist.Videos.Add(video);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Playlist>> GetByFilterAsync(PlaylistFilterDto dto)
        {
            var query = _dbContext.Playlists
                .Include(p => p.Creator)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name) || !string.IsNullOrEmpty(dto.Description))
            {
                var searchTerm = dto.Name ?? dto.Description;
                query = query.Where(playlist =>
                    EF.Functions.ILike(playlist.Name, $"%{searchTerm}%") ||
                    (playlist.Description != null && EF.Functions.ILike(playlist.Description, $"%{searchTerm}%"))
                );
            }

            if (!string.IsNullOrEmpty(dto.CreatorId))
            {
                query = query.Where(playlist => playlist.CreatorId == dto.CreatorId);
            }

            if (dto.FromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= dto.FromDate.Value);
            }

            if (dto.ToDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= dto.ToDate.Value);
            }

            query = dto.SortBy?.ToLower() switch
            {
                "name" => dto.SortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "updatedat" => dto.SortDescending
                    ? query.OrderByDescending(p => p.UpdatedAt)
                    : query.OrderBy(p => p.UpdatedAt),
                _ => dto.SortDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt)
            };

            query = query
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize);

            return await query.ToListAsync();
        }

        public async Task<int> GetFilteredCountAsync(PlaylistFilterDto dto)
        {
            var query = _dbContext.Playlists.AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name) || !string.IsNullOrEmpty(dto.Description))
            {
                var searchTerm = dto.Name ?? dto.Description;
                query = query.Where(playlist =>
                    EF.Functions.ILike(playlist.Name, $"%{searchTerm}%") ||
                    (playlist.Description != null && EF.Functions.ILike(playlist.Description, $"%{searchTerm}%"))
                );
            }

            if (!string.IsNullOrEmpty(dto.CreatorId))
            {
                query = query.Where(playlist => playlist.CreatorId == dto.CreatorId);
            }

            if (dto.FromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= dto.FromDate.Value);
            }

            if (dto.ToDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= dto.ToDate.Value);
            }

            return await query.CountAsync();
        }
    }
}
