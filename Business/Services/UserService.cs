using Business.DTOs;
using Business.Mappers;
using Infra.Repository;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        //TODO: Use DI
        var mapper = new UserMapper();
        var users = await _userRepository.GetAllUsers();
        return mapper.Map(users);
    }
}