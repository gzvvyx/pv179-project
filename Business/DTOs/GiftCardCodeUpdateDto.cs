namespace Business.DTOs;

public class GiftCardCodeUpdateDto
{
    public required bool Used { get; set; }
    public int? OrderId { get; set; }
}
