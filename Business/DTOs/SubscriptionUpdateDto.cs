using DAL.Models.Enums;

namespace Business.DTOs;

public class SubscriptionUpdateDto
{
    public required int Id { get; set; }
    public bool? Active { get; set; }
    public SubscriptionTimeframe? Timeframe { get; set; }
    public DateTime? LastRenewedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
