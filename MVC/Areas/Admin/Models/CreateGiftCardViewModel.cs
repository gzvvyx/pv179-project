using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class CreateGiftCardViewModel
{
    [Required]
    [Display(Name = "Price Reduction")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price reduction must be greater than 0")]
    public decimal PriceReduction { get; set; }

    [Required]
    [Display(Name = "Valid From")]
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Valid To")]
    public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddMonths(1);
}

