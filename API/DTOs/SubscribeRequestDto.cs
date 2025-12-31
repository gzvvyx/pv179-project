using System.Text.Json.Serialization;
using DAL.Models.Enums;

namespace API.DTOs;

public class SubscribeRequestDto
{
    [JsonRequired]
    public required string CreatorId { get; set; }

    [JsonRequired]
    public required SubscriptionTimeframe Timeframe { get; set; }

    public string? GiftCardCode { get; set; }
}

