using DAL.Models;

namespace Business.DTOs;

public class OrderDto
{
    public required int Id { get; set; }
    public required string OrdererId { get; set; }
    public required string CreatorId { get; set; }
    public required decimal Amount { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
