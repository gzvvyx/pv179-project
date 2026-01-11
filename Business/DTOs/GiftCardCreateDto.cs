namespace Business.DTOs;

public class GiftCardCreateDto
{
    public required decimal PriceReduction { get; set; }
    public required DateTime ValidFrom { get; set; }
    public required DateTime ValidTo { get; set; }
    public ICollection<GiftCardCodeDto> GiftCardCodes { get; set; } = new List<GiftCardCodeDto>();
}
