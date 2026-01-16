using DAL.Models.Enums;

namespace pv179.Models;

public class SubscriptionViewModel
{
    public required int Id { get; set; }
    public required string OrdererUserName { get; set; }
    public required string CreatorId { get; set; }
    public required string CreatorUserName { get; set; }
    public required bool Active { get; set; }
    public required SubscriptionTimeframe Timeframe { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required DateTime SubscribedAt { get; set; }
    public required DateTime LastRenewedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
