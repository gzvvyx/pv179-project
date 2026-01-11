using DAL.Models.Enums;

namespace Business.DTOs;

public class ProcessPaymentDto
{
    public required string OrdererId { get; set; }
    public required string CreatorId { get; set; }
    public required SubscriptionTimeframe Timeframe { get; set; }
    public string? GiftCardCode { get; set; }
}

public class PaymentResultDto
{
    public required bool Success { get; set; }
    public required OrderDto Order { get; set; }
    public SubscriptionDto? Subscription { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public string? GiftCardCodeUsed { get; set; }
}

