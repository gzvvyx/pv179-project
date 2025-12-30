using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class GiftCardCodeMapper
{
    public partial List<GiftCardCodeDto> Map(List<GiftCardCode> giftCardCodes);
    public partial GiftCardCodeDto Map(GiftCardCode giftCardCode);
}
