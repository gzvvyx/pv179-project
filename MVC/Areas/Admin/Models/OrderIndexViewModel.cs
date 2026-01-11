using DAL.Models.Enums;

namespace MVC.Areas.Admin.Models;

public class OrderIndexViewModel
{
    public int Id { get; set; }
    public string OrdererUserName { get; set; } = null!;
    public string OrdererId { get; set; } = null!;
    public string CreatorUserName { get; set; } = null!;
    public string CreatorId { get; set; } = null!;
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

