using API.DTOs;
using Infra.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class UserRequestMapper
{
    [MapperIgnoreTarget(nameof(UserFilterDto.PageNumber))]
    [MapperIgnoreTarget(nameof(UserFilterDto.PageSize))]
    [MapperIgnoreTarget(nameof(UserFilterDto.SortBy))]
    [MapperIgnoreTarget(nameof(UserFilterDto.SortDescending))]
    public partial UserFilterDto ToFilterDto(GetUsersRequestDto dto);
    
    public partial UserFilterDto ToFilterDto(GetUsersPagedRequestDto dto);
}
