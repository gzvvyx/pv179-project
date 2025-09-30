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
}