using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class OrderCreateDto
{
    [Required]
    public string OrdererId { get; set; }
    [Required]
    public string CreatorId { get; set; }
    public decimal? Amount { get; set; }
}
