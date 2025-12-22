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
                .ToListAsync();
        }

        public Task<Playlist?> GetByIdAsync(int id)
        {
            return _dbContext.Playlists
                .Include(playlist => playlist.Creator)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task CreateAsync(Playlist playlist)
        {
            if (playlist.Creator is not null)
            {
                _dbContext.Attach(playlist.Creator);
            }

            await _dbContext.Playlists.AddAsync(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {

            if (playlist.Creator is not null)
            {
                _dbContext.Attach(playlist.Creator);
            }

            _dbContext.Playlists.Update(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Playlist>> GetByFilterAsync(PlaylistFilterDto dto)
        {
            var query = _dbContext.Playlists.AsQueryable();
            if (!string.IsNullOrEmpty(dto.CreatorId))
            {
                query = query.Where(playlist => playlist.CreatorId == dto.CreatorId);
            }
            if (!string.IsNullOrEmpty(dto.Name))
            {
                query = query.Where(playlist => playlist.Name.Contains(dto.Name));
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                query = query.Where(playlist => playlist.Description.Contains(dto.Description));
            }

            return await query.ToListAsync();
        }
    }
}
