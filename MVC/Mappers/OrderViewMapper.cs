using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class OrderViewMapper
{
    public partial List<OrderViewModel> MapToViewModelList(List<OrderDto> orders);
    
    [MapProperty(nameof(OrderDto.Orderer.UserName), nameof(OrderViewModel.OrdererUserName))]
    [MapProperty(nameof(OrderDto.Creator.UserName), nameof(OrderViewModel.CreatorUserName))]
    public partial OrderViewModel MapToViewModel(OrderDto order);
    
    [MapProperty(nameof(OrderDto.Orderer.UserName), nameof(OrderDetailsViewModel.OrdererUserName))]
    [MapProperty(nameof(OrderDto.Creator.UserName), nameof(OrderDetailsViewModel.CreatorUserName))]
    public partial OrderDetailsViewModel MapToDetailsViewModel(OrderDto order);
}
