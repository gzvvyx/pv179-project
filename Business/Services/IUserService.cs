using Business.DTOs;

namespace Business.Services;

public interface IUserService
{
    public Task<List<UserDto>> GetAllAsync();
}