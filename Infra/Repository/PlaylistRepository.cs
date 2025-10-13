using DAL.Data;
using DAL.Models;
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
    }
}
