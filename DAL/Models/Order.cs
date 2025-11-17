using DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Order : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Orderer))]
    public required string OrdererId { get; init; }

    [Required]
    [ForeignKey(nameof(Creator))]
    public required string CreatorId { get; init; }

    [Required]
    public required decimal Amount { get; set; }

    [Required]
    public required OrderStatus Status { get; set; }

    [Required]
    [InverseProperty(nameof(User.OrdersPlaced))]
    public required User Orderer { get; set; }

    [Required]
    [InverseProperty(nameof(User.OrdersReceived))]
    public required User Creator  { get; set; }
}
