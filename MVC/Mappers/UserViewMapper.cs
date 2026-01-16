using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class UserViewMapper
{
    [MapperIgnoreTarget(nameof(UserViewModel.IsCurrentUser))]
    [MapperIgnoreTarget(nameof(UserViewModel.IsSubscribed))]
    [MapperIgnoreTarget(nameof(UserViewModel.Videos))]
    [MapperIgnoreTarget(nameof(UserViewModel.Playlists))]
    public partial UserViewModel MapToViewModel(UserDto userDto);
    
    [MapperIgnoreTarget(nameof(UserEditViewModel.NewPassword))]
    [MapperIgnoreTarget(nameof(UserEditViewModel.ConfirmPassword))]
    public partial UserEditViewModel MapToEditViewModel(UserDto userDto);
}
