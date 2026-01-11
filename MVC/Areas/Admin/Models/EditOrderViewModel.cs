using Business.DTOs;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class EditOrderViewModel
{
    public OrderDto Order { get; set; } = null!;
    public string OrdererName { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    public decimal? Amount { get; set; }
    public OrderStatus? Status { get; set; }
    public List<SelectListItem> Statuses { get; set; } = new();
}

