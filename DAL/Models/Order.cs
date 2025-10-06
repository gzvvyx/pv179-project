using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public enum OrderStatus
{
    Pending,
    Failed,
    Completed
}

public class Order
{
    public required int Id { get; init; }
    [ForeignKey(nameof(Orderer))]
    public required string OrdererId { get; init; }
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; init; }
    public required DateTime OrderDate { get; init; }
    public required decimal Amount { get; init; }
    public required OrderStatus Status { get; init; }
    
    public required User Orderer { get; init; }
    public required User Creator  { get; init; }
}