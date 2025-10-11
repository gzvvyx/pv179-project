using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public enum OrderStatus
{
    Pending,
    Failed,
    Completed
}

public class Order : BaseEntity
{
    [ForeignKey(nameof(Orderer))]
    public required string OrdererId { get; init; }
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; init; }
    public required decimal Amount { get; init; }
    public required OrderStatus Status { get; init; }
    
    [InverseProperty(nameof(User.OrdersPlaced))]
    public required User Orderer { get; set; }
    
    [InverseProperty(nameof(User.OrdersReceived))]
    public required User Creator  { get; set; }
}
