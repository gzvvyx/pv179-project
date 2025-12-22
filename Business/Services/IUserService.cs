using Business.DTOs;
using Infra.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<(IdentityResult Result, UserDto? User)> CreateAsync(UserCreateDto dto);
    Task<(IdentityResult Result, UserDto? User)> UpdateAsync(string id, UserUpdateDto dto);
    Task<IdentityResult> DeleteAsync(string id);
    Task<List<UserDto>> GetByFilterAsync(UserFilterDto dto);
}
