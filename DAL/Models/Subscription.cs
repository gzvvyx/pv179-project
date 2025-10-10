using System.ComponentModel.DataAnnotations.Schema;

public enum SubscriptionTimeframe
{
    Month,
    HalfYear,
    Year
}


namespace DAL.Models
{
    public class Subscription
    {
        public required int Id { get; set; }
        [ForeignKey(nameof(Orderer))]
        public required string OrdererId { get; set; }
        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required bool Active { get; set; }
        public required SubscriptionTimeframe Timeframe { get; set; }
        public required DateTime SubscribedAt { get; set; }
        public required DateTime LastRenewedAt { get; set; }
        public required DateTime ExpiresAt { get; set; }

        public required User Orderer { get; set; }
        public required User Creator { get; set; }
    }
}
