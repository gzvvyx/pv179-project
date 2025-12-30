using API.DTOs;
using Business.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class OrderRequestMapper
{
    public partial OrderCreateDto ToBusinessDto(OrderCreateRequestDto dto);
    public partial OrderUpdateDto ToBusinessDto(OrderUpdateRequestDto dto, int Id);
}


