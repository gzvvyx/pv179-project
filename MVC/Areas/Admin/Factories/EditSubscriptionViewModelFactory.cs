using Business.DTOs;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditSubscriptionViewModelFactory : IEditSubscriptionViewModelFactory
{
    public Task<EditSubscriptionViewModel> CreateAsync(SubscriptionDto subscription, SubscriptionTimeframe? selectedTimeframe = null)
    {
        var timeframe = selectedTimeframe ?? subscription.Timeframe;
        
        var model = new EditSubscriptionViewModel
        {
            Subscription = subscription,
            OrdererName = subscription.Orderer.UserName,
            CreatorName = subscription.Creator.UserName,
            Active = subscription.Active,
            Timeframe = timeframe,
            Timeframes = CreateTimeframeSelectList(timeframe),
            LastRenewedAt = subscription.LastRenewedAt,
            ExpiresAt = subscription.ExpiresAt
        };
        
        return Task.FromResult(model);
    }

    public Task PopulateViewModelAsync(EditSubscriptionViewModel model, SubscriptionDto subscription)
    {
        var timeframe = model.Timeframe ?? subscription.Timeframe;
        
        model.Subscription = subscription;
        model.OrdererName = subscription.Orderer.UserName;
        model.CreatorName = subscription.Creator.UserName;
        model.Active = subscription.Active;
        model.Timeframes = CreateTimeframeSelectList(timeframe);
        
        return Task.CompletedTask;
    }

    private List<SelectListItem> CreateTimeframeSelectList(SubscriptionTimeframe selectedTimeframe)
    {
        return Enum.GetValues(typeof(SubscriptionTimeframe))
            .Cast<SubscriptionTimeframe>()
            .Select(t => new SelectListItem
            {
                Text = t.ToString(),
                Value = t.ToString(),
                Selected = selectedTimeframe == t
            }).ToList();
    }
}

