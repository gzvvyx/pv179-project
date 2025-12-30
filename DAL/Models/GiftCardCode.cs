using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;
public class GiftCardCode
{
    [Key]
    [Required]
    [MaxLength(20)]
    public required string Code { get; set; }

    [Required]
    public required bool Used { get; set; } = false;

    [Required]
    public required int GiftCardId { get; set; }

    [Required]
    public required GiftCard GiftCard { get; set; }

    [ForeignKey(nameof(Order))]
    public int? OrderId { get; set; }

    public Order? Order { get; set; }
}
