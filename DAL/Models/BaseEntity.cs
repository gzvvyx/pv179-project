using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public abstract class BaseEntity
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    [Required]
    public required DateTime UpdatedAt { get; set; }
}
