using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class UserViewMapper
{
    public partial UserViewModel MapToViewModel(UserDto userDto);
    public partial UserEditViewModel MapToEditViewModel(UserDto userDto);
}
