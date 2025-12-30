using Business.DTOs;

namespace MVC.Areas.Admin.Models;

public class OrderWithUsersViewModel
{
    public OrderDto Order { get; set; } = null!;
    public string OrdererName { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
}

