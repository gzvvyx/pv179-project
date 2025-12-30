using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class CreateGiftCardCodeViewModel
{
    [Required]
    [Display(Name = "Code")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Code must be between 1 and 50 characters")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Used")]
    public bool Used { get; set; } = false;

    [Required]
    public int GiftCardId { get; set; }
}

