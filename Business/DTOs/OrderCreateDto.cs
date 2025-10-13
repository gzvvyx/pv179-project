using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class OrderCreateDto
{
    [Required]
    public string OrdererId { get; set; }
    [Required]
    public string CreatorId { get; set; }
    [Required]
    public required decimal Amount { get; set; }
}
