namespace MVC.Areas.Admin.Models;

public class GiftCardIndexViewModel
{
    public int Id { get; set; }
    public decimal PriceReduction { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int CodesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

