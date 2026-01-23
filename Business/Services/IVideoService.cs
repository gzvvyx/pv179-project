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
    Task<ErrorOr<Success>> AddCategoryAsync(int videoId, int categoryId, bool isPrimary = false);
    Task<ErrorOr<Success>> RemoveCategoryAsync(int videoId, int categoryId);
    Task<ErrorOr<Success>> SetPrimaryCategoryAsync(int videoId, int categoryId);
}

