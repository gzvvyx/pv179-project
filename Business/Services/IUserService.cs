using Business.DTOs;
using ErrorOr;
using Infra.DTOs;

namespace Business.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<UserDetailsDto?> GetDetailsByIdAsync(string id);
    Task<ErrorOr<UserDto>> CreateAsync(UserCreateDto dto);
    Task<ErrorOr<UserDto>> UpdateAsync(string id, UserUpdateDto dto);
    Task<ErrorOr<Success>> DeleteAsync(string id);
    Task<List<UserDto>> GetByFilterAsync(UserFilterDto dto);
    Task<PagedResultDto<UserDto>> GetByFilterPagedAsync(UserFilterDto dto);
}
