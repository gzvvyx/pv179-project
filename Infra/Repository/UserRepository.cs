using DAL.Models;
using Infra.DTOs;
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

    public Task<List<User>> GetAllAsync()
    {
        return _userManager.Users.ToListAsync();
    }

    public Task<User?> GetByIdAsync(string id)
    {
        return _userManager.FindByIdAsync(id);
    }

    public Task<IdentityResult> CreateAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> UpdateAsync(User user)
    {
        return _userManager.UpdateAsync(user);
    }

    public Task<IdentityResult> DeleteAsync(User user)
    {
        return _userManager.DeleteAsync(user);
    }

    public async Task<List<User>> GetByFilterAsync(UserFilterDto dto)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(dto.UserName) || !string.IsNullOrEmpty(dto.Email))
        {
            var searchTerm = dto.UserName ?? dto.Email;
            query = query.Where(user =>
                (user.UserName != null && EF.Functions.ILike(user.UserName, $"%{searchTerm}%")) ||
                (user.Email != null && EF.Functions.ILike(user.Email, $"%{searchTerm}%"))
            );
        }

        query = dto.SortBy?.ToLower() switch
        {
            "email" => dto.SortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            _ => dto.SortDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName)
        };

        query = query
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize);

        return await query.ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(UserFilterDto dto)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(dto.UserName) || !string.IsNullOrEmpty(dto.Email))
        {
            var searchTerm = dto.UserName ?? dto.Email;
            query = query.Where(user =>
                (user.UserName != null && EF.Functions.ILike(user.UserName, $"%{searchTerm}%")) ||
                (user.Email != null && EF.Functions.ILike(user.Email, $"%{searchTerm}%"))
            );
        }

        return await query.CountAsync();
    }
}
