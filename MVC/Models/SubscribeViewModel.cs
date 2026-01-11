using System.ComponentModel.DataAnnotations;
using DAL.Models.Enums;

namespace pv179.Models;

public class SubscribeViewModel
{
    [Required]
    public required string CreatorId { get; set; }
    
    public required string CreatorName { get; set; }
    
    [Required]
    [Display(Name = "Subscription Plan")]
    public SubscriptionTimeframe Timeframe { get; set; } = SubscriptionTimeframe.Month;
    
    [Display(Name = "Gift Card Code")]
    [MaxLength(20)]
    public string? GiftCardCode { get; set; }
    
    public decimal MonthlyPrice { get; set; }
}

public class SubscribeResultViewModel
{
    public bool Success { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public string? GiftCardCodeUsed { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
}

