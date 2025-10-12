using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    
    public UserRepository(UserManager<User> userManager) 
    {
        _userManager = userManager;
    }


    public Task<List<User>> GetAllUsers()
    {
        return _userManager.Users.ToListAsync();
    }

    public Task<User?> GetUserByIdAsync(string id)
    {
        return _userManager.FindByIdAsync(id);
    }

    public Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> UpdateUserAsync(User user)
    {
        return _userManager.UpdateAsync(user);
    }

    public Task<IdentityResult> DeleteUserAsync(User user)
    {
        return _userManager.DeleteAsync(user);
    }
}
