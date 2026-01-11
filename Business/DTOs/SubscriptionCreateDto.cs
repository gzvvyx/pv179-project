using DAL.Models.Enums;

namespace Business.DTOs;

public class SubscriptionCreateDto
{
    public required string OrdererId { get; set; }
    public required string CreatorId { get; set; }
    public bool? Active { get; set; }
    public SubscriptionTimeframe? Timeframe { get; set; }
}
