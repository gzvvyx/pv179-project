using Business.DTOs;

namespace MVC.Areas.Admin.Models;

public class SubscriptionWithUsersViewModel
{
    public SubscriptionDto Subscription { get; set; } = null!;
    public string OrdererName { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
}

