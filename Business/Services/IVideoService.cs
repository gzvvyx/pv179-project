using Business.DTOs;
using ErrorOr;
using Infra.DTOs;

namespace Business.Services;

public interface IVideoService
{
    Task<List<VideoDto>> GetAllAsync();
    Task<VideoDto?> GetByIdAsync(int id);
    Task<ErrorOr<VideoDto>> CreateAsync(VideoCreateDto dto);
    Task<ErrorOr<VideoDto>> UpdateAsync(VideoUpdateDto dto);
    Task<ErrorOr<Success>> DeleteAsync(int id);
    Task<List<VideoDto>> GetByFilterAsync(VideoFilterDto dto);
    Task<PagedResultDto<VideoDto>> GetByFilterPagedAsync(VideoFilterDto dto);
}

