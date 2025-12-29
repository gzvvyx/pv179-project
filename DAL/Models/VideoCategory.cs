using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class VideoCategory : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Video))]
    public required int VideoId { get; set; }

    [Required]
    [ForeignKey(nameof(Category))]
    public required int CategoryId { get; set; }

    [Required]
    public required bool IsPrimary { get; set; }

    [InverseProperty(nameof(Models.Video.VideoCategories))]
    public Video? Video { get; set; }

    [InverseProperty(nameof(Models.Category.VideoCategories))]
    public Category? Category { get; set; }
}
