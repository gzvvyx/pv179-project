using System.Text.Json.Serialization;

namespace API.DTOs;

public class ValidateGiftCardRequestDto
{
    [JsonRequired]
    public required string Code { get; set; }
}

