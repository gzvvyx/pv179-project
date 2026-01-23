namespace Business.DTOs;

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
