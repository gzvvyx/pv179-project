using Business.DTOs;
using pv179.Models;
using Riok.Mapperly.Abstractions;

namespace pv179.Mappers;

[Mapper]
public partial class SubscriptionViewMapper
{
    public partial List<SubscriptionViewModel> MapToViewModelList(List<SubscriptionDto> subscriptions);

    [MapProperty(nameof(SubscriptionDto.Orderer.UserName), nameof(SubscriptionViewModel.OrdererUserName))]
    [MapProperty(nameof(SubscriptionDto.Creator.UserName), nameof(SubscriptionViewModel.CreatorUserName))]
    public partial SubscriptionViewModel MapToViewModel(SubscriptionDto subscription);
}
