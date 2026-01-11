using System.Text.Json.Serialization;
using DAL.Models.Enums;

namespace API.DTOs;

public class OrderUpdateRequestDto
{
    [JsonRequired]
    public required decimal Amount { get; set; }

    [JsonRequired]
    public required OrderStatus Status { get; set; }
}


