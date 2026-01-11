using Business.DTOs;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public class EditOrderViewModelFactory : IEditOrderViewModelFactory
{
    public Task<EditOrderViewModel> CreateAsync(OrderDto order, OrderStatus? selectedStatus = null)
    {
        var status = selectedStatus ?? order.Status;
        
        var model = new EditOrderViewModel
        {
            Order = order,
            OrdererName = order.Orderer.UserName,
            CreatorName = order.Creator.UserName,
            Amount = order.Amount,
            Status = status,
            Statuses = CreateStatusSelectList(status)
        };
        
        return Task.FromResult(model);
    }

    public Task PopulateViewModelAsync(EditOrderViewModel model, OrderDto order)
    {
        var status = model.Status ?? order.Status;
        
        model.Order = order;
        model.OrdererName = order.Orderer.UserName;
        model.CreatorName = order.Creator.UserName;
        model.Statuses = CreateStatusSelectList(status);
        
        return Task.CompletedTask;
    }

    private List<SelectListItem> CreateStatusSelectList(OrderStatus selectedStatus)
    {
        return Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString(),
                Selected = selectedStatus == s
            }).ToList();
    }
}

