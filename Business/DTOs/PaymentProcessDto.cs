using DAL.Models.Enums;

namespace Business.DTOs;

public class PaymentProcessDto
{
    public required string OrdererId { get; set; }
    public required string CreatorId { get; set; }
    public required SubscriptionTimeframe Timeframe { get; set; }
    public string? GiftCardCode { get; set; }
}
