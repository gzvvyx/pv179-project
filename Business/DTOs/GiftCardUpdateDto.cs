namespace Business.DTOs;

public class GiftCardUpdateDto
{
    public decimal? PriceReduction { get; set; }
    public ICollection<GiftCardCodeDto> GiftCardCodes { get; set; } = new List<GiftCardCodeDto>();
}
