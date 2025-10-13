using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class OrderMapper
{
    public partial List<OrderDto> Map(List<Order> orders);
    public partial OrderDto Map(Order order);
}
