using DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace Infra.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserByIdAsync(string id);
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<IdentityResult> UpdateUserAsync(User user);
    Task<IdentityResult> DeleteUserAsync(User user);
}
