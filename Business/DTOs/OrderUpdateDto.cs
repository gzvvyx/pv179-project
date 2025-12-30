using DAL.Models;
using DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class OrderUpdateDto
{    
    public required int Id { get; set; }
    public decimal? Amount { get; set; }
    public OrderStatus? Status { get; set; }
    public int? GiftCardCodeId { get; set; }
    public GiftCardCode? GiftCardCode { get; set; }

}