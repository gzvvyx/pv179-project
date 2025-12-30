using Business.DTOs;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class EditSubscriptionViewModel
{
    public SubscriptionDto Subscription { get; set; } = null!;
    public string OrdererName { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    public bool Active { get; set; }
    public SubscriptionTimeframe? Timeframe { get; set; }
    public List<SelectListItem> Timeframes { get; set; } = new();
    public DateTime? LastRenewedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

