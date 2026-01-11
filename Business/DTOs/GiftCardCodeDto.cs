using DAL.Models;

namespace Business.DTOs;

public class GiftCardCodeDto
{
    public required string Code { get; set; }
    public required bool Used { get; set; }
    public required int GiftCardId { get; set; }
    public required GiftCard GiftCard { get; set; }
    public int? OrderId { get; set; }
}
