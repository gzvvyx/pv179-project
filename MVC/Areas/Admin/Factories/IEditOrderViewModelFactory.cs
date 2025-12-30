using Business.DTOs;
using DAL.Models.Enums;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public interface IEditOrderViewModelFactory
{
    Task<EditOrderViewModel> CreateAsync(OrderDto order, OrderStatus? selectedStatus = null);
    Task PopulateViewModelAsync(EditOrderViewModel model, OrderDto order);
}

