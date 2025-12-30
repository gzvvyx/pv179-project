using Business.DTOs;

namespace MVC.Areas.Admin.Models;

public class EditGiftCardViewModel
{
    public int Id { get; set; }
    public decimal PriceReduction { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public List<GiftCardCodeDto> Codes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

