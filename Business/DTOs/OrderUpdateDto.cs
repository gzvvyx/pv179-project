using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class OrderUpdateDto
{    
    public decimal? Amount { get; set; }
    public OrderStatus? Status { get; set; }
}