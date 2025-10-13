using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class SubscriptionMapper
{
    public partial List<SubscriptionDto> Map(List<Subscription> subscriptions);
    public partial SubscriptionDto Map(Subscription subscription);
}
