using Business.DTOs;
using DAL.Models.Enums;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public interface IEditSubscriptionViewModelFactory
{
    Task<EditSubscriptionViewModel> CreateAsync(SubscriptionDto subscription, SubscriptionTimeframe? selectedTimeframe = null);
    Task PopulateViewModelAsync(EditSubscriptionViewModel model, SubscriptionDto subscription);
}

