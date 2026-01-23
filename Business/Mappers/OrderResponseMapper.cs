using Business.DTOs;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class OrderResponseMapper
{
    public OrderWithUsersDto ToOrderWithUsersDto(OrderDto order)
    {
        return new OrderWithUsersDto
        {
            Order = order,
            OrdererName = order.Orderer?.UserName ?? "Unknown",
            CreatorName = order.Creator?.UserName ?? "Unknown"
        };
    }
}
