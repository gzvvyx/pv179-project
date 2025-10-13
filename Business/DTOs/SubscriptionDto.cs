using DAL.Models;

namespace Business.DTOs;

public class SubscriptionDto
{
    public required int Id { get; set; }
    public required string OrdererId { get; set; }
    public required string CreatorId { get; set; }
    public required bool Active { get; set; }
    public required SubscriptionTimeframe Timeframe { get; set; }
    public required DateTime SubscribedAt { get; set; }
    public required DateTime LastRenewedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
