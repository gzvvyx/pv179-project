using DAL.Models.Enums;

namespace MVC.Areas.Admin.Models;

public class SubscriptionIndexViewModel
{
    public int Id { get; set; }
    public string OrdererUserName { get; set; } = null!;
    public string OrdererId { get; set; } = null!;
    public string CreatorUserName { get; set; } = null!;
    public string CreatorId { get; set; } = null!;
    public bool Active { get; set; }
    public SubscriptionTimeframe Timeframe { get; set; }
    public DateTime SubscribedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

