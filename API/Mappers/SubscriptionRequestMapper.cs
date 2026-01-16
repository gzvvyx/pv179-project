using API.DTOs;
using Business.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class SubscriptionRequestMapper
{
    public partial PaymentProcessDto ToPaymentProcessDto(SubscribeRequestDto dto, string ordererId);
}
