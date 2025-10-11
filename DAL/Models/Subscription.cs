using System.ComponentModel.DataAnnotations.Schema;

public enum SubscriptionTimeframe
{
    Month,
    HalfYear,
    Year
}


namespace DAL.Models
{
    public class Subscription : BaseEntity
    {
        [ForeignKey(nameof(Orderer))]
        public required string OrdererId { get; set; }
        [ForeignKey(nameof(Creator))]
        public required string CreatorId { get; set; }
        public required bool Active { get; set; }
        public required SubscriptionTimeframe Timeframe { get; set; }
        public required DateTime SubscribedAt { get; set; }
        public required DateTime LastRenewedAt { get; set; }
        public required DateTime ExpiresAt { get; set; }

        [InverseProperty(nameof(User.SubscriptionsPurchased))]
        public required User Orderer { get; set; }

        [InverseProperty(nameof(User.SubscriptionsOffered))]
        public required User Creator { get; set; }
    }
}
