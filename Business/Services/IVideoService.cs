using Business.DTOs;
using Infra.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IVideoService
{
    Task<List<VideoDto>> GetAllAsync();
    Task<VideoDto?> GetByIdAsync(int id);
    Task<(IdentityResult Result, VideoDto? Video)> CreateAsync(VideoCreateDto dto);
    Task<(IdentityResult Result, VideoDto? Video)> UpdateAsync(int id, VideoUpdateDto dto);
    Task<IdentityResult> DeleteAsync(int id);
    Task<List<VideoDto>> GetByFilterAsync(VideoFilterDto dto);
    Task<PagedResultDto<VideoDto>> GetByFilterPagedAsync(VideoFilterDto dto);
}

