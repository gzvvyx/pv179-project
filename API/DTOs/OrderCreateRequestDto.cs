using System.Text.Json.Serialization;

namespace API.DTOs;

public class OrderCreateRequestDto
{
    [JsonRequired]
    public required string OrdererId { get; set; }

    [JsonRequired]
    public required string CreatorId { get; set; }

    [JsonRequired]
    public required decimal Amount { get; set; }
}


