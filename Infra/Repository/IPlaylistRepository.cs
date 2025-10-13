using DAL.Models;

namespace Infra.Repository
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetAllAsync();
        Task<Playlist?> GetByIdAsync(int id);
        Task CreateAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(Playlist playlist);
    }
}
