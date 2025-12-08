using DAL.Models;
using Infra.DTOs;

namespace Infra.Repository;

public interface IVideoRepository
{
    Task<List<Video>> GetAllAsync();
    Task<Video?> GetByIdAsync(int id);
    Task AddAsync(Video video);
    Task UpdateAsync(Video video);
    Task DeleteAsync(Video video);
    Task<List<Video>> GetByFilterAsync(VideoFilterDto dto);
}

