using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class SubscriptionViewMapper
{
    public partial List<SubscriptionViewModel> MapToViewModelList(List<SubscriptionDto> subscriptions);

    [MapProperty(nameof(SubscriptionDto.Orderer.UserName), nameof(SubscriptionViewModel.OrdererUserName))]
    [MapProperty(nameof(SubscriptionDto.Creator.Id), nameof(SubscriptionViewModel.CreatorId))]
    [MapProperty(nameof(SubscriptionDto.Creator.UserName), nameof(SubscriptionViewModel.CreatorUserName))]
    [MapperIgnoreSource(nameof(SubscriptionDto.CreatedAt))]
    public partial SubscriptionViewModel MapToViewModel(SubscriptionDto subscription);
}
