namespace Business.DTOs;
public class GiftCardDto
{
    public required int Id { get; set; }
    public required decimal PriceReduction { get; set; }
    public required DateTime ValidFrom { get; set; }
    public required DateTime ValidTo { get; set; }
    public ICollection<GiftCardCodeDto> Codes { get; set; } = new List<GiftCardCodeDto>();
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
