using DAL.Models;
using Infra.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Infra.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IdentityResult> DeleteAsync(User user);
    Task<List<User>> GetByFilterAsync(UserFilterDto dto);
    Task<int> GetFilteredCountAsync(UserFilterDto dto);
}
