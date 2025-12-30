using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;
public class GiftCard : BaseEntity
{
    [Required]
    public required decimal PriceReduction { get; set; }

    [Required]
    public required DateTime ValidFrom { get; set; }

    [Required]
    public required DateTime ValidTo { get; set; }

    public ICollection<GiftCardCode> Codes { get; set; } = new List<GiftCardCode>();
}
