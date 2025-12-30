using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class CreateCategoryViewModel
{
    [Required]
    [Display(Name = "Category Name")]
    [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
}

