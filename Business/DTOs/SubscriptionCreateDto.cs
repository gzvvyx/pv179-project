using DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class SubscriptionCreateDto
{
    [Required]
    public required string OrdererId { get; set; }
    [Required]
    public required string CreatorId { get; set; }
    [Required] 
    public required bool Active { get; set; }
    [Required]
    public required SubscriptionTimeframe Timeframe { get; set; }
}
