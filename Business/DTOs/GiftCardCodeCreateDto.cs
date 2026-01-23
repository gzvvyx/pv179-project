using DAL.Models;

namespace Business.DTOs;

public class GiftCardCodeCreateDto
{
    public required string Code { get; set; }
    public required bool Used { get; set; }
    public required int GiftCardId { get; set; }
}
