using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using Business.Validators;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.DTOs;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<UserCreateDto> _createValidator;
    private readonly IValidator<UserUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly UserMapper _mapper = new();

    public UserService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        AppDbContext dbContext,
        IValidator<UserCreateDto> createValidator,
        IValidator<UserUpdateDto> updateValidator)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _dbContext = dbContext;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map(users);
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : _mapper.Map(user);
    }

    public async Task<ErrorOr<UserDto>> CreateAsync(UserCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PricePerMonth = dto.PricePerMonth
        };

        var result = await _userRepository.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return result.ToErrors();
        }

        await _dbContext.SaveChangesAsync();

        return _mapper.Map(user);
    }

    public async Task<ErrorOr<UserDto>> UpdateAsync(string id, UserUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Error.NotFound();
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

        if (dto.PricePerMonth != user.PricePerMonth)
        {
            user.PricePerMonth = dto.PricePerMonth;
        }

        var updateResult = await _userRepository.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return updateResult.ToErrors();
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    return removePasswordResult.Errors
                        .Select(error => Error.Validation(nameof(dto.NewPassword), error.Description))
                        .ToList();
                }
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                return addPasswordResult.Errors
                    .Select(error => Error.Validation(nameof(dto.NewPassword), error.Description))
                    .ToList();
            }
        }

        if (dto.Roles != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = dto.Roles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(dto.Roles).ToList();

            // Add new roles
            foreach (var role in rolesToAdd)
            {
                var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    return addRoleResult.ToErrors();
                }
            }

            // Remove roles
            foreach (var role in rolesToRemove)
            {
                var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, role);
                if (!removeRoleResult.Succeeded)
                {
                    return removeRoleResult.ToErrors();
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        return _mapper.Map(user);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return Error.NotFound();
        }

        var result = await _userRepository.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return result.ToErrors();
        }

        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<List<UserDto>> GetByFilterAsync(UserFilterDto dto)
    {
        var users = await _userRepository.GetByFilterAsync(dto);
        return _mapper.Map(users);
    }

    public async Task<PagedResultDto<UserDto>> GetByFilterPagedAsync(UserFilterDto dto)
    {
        var users = await _userRepository.GetByFilterAsync(dto);
        var totalCount = await _userRepository.GetFilteredCountAsync(dto);

        return new PagedResultDto<UserDto>
        {
            Items = _mapper.Map(users),
            TotalCount = totalCount,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize
        };
    }
}
