using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class UserMapper
{
    public partial List<UserDto> Map(List<User> users);
    public partial UserDto Map(User user);
}
