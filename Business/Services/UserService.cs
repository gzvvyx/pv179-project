using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.DTOs;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _mapper = new();

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllUsers();
        return _mapper.Map(users);
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return user is null ? null : _mapper.Map(user);
    }

    public async Task<(IdentityResult Result, UserDto? User)> CreateAsync(UserCreateDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userRepository.CreateUserAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return (result, null);
        }

        return (result, _mapper.Map(user));
    }

    public async Task<(IdentityResult Result, UserDto? User)> UpdateAsync(string id, UserUpdateDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = $"User with id '{id}' was not found."
            }), null);
        }

        if (!string.IsNullOrWhiteSpace(dto.UserName) &&
            !string.Equals(user.UserName, dto.UserName, StringComparison.Ordinal))
        {
            user.UserName = dto.UserName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = dto.Email;
        }

        var result = await _userRepository.UpdateUserAsync(user);

        if (!result.Succeeded)
        {
            return (result, null);
        }

        return (result, _mapper.Map(user));
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = $"User with id '{id}' was not found."
            });
        }

        return await _userRepository.DeleteUserAsync(user);
    }

    public async Task<List<UserDto>> GetByFilterAsync(UserFilterDto dto)
    {
        var users = await _userRepository.GetUsersByFilterAsync(dto);
        return _mapper.Map(users);
    }
}
