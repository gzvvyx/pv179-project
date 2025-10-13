using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class SubscriptionUpdateDto
{
    public bool? Active { get; set; }
    public SubscriptionTimeframe? Timeframe { get; set; }
    public DateTime? LastRenewedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
