namespace Business.DTOs;

public class GiftCardUpdateDto
{
    public required int Id { get; set; }
    public decimal? PriceReduction { get; set; }
    public ICollection<GiftCardCodeDto> GiftCardCodes { get; set; } = new List<GiftCardCodeDto>();
}
