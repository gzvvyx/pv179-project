using DAL.Models.Enums;

namespace pv179.Models;

public class OrderViewModel
{
    public required int Id { get; set; }
    public required string OrdererUserName { get; set; }
    public required string CreatorId { get; set; }
    public required string CreatorUserName { get; set; }
    public required decimal Amount { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}

public class OrderDetailsViewModel
{
    public required int Id { get; set; }
    public required string OrdererUserName { get; set; }
    public required string CreatorUserName { get; set; }
    public required decimal Amount { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
