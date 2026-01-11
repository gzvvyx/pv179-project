using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [InverseProperty(nameof(VideoCategory.Category))]
    public ICollection<VideoCategory> VideoCategories { get; set; } = new List<VideoCategory>();
}
