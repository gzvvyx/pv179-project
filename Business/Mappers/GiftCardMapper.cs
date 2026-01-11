using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class GiftCardMapper
{
    public partial List<GiftCardDto> Map(List<GiftCard> giftCards);
    public partial GiftCardDto Map(GiftCard giftCard);
}
