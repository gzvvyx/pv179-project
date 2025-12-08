using DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Subscription : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Orderer))]
    public required string OrdererId { get; set; }

    [Required]
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; set; }

    [Required]
    public required bool Active { get; set; }

    [Required]
    public required SubscriptionTimeframe Timeframe { get; set; }

    [Required]
    public required DateTime SubscribedAt { get; set; }

    [Required]
    public required DateTime LastRenewedAt { get; set; }

    [Required]
    public required DateTime ExpiresAt { get; set; }

    [Required]
    [InverseProperty(nameof(User.SubscriptionsPurchased))]
    public required User Orderer { get; set; }

    [Required]
    [InverseProperty(nameof(User.SubscriptionsOffered))]
    public required User Creator { get; set; }
}
