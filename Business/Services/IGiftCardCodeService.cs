using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface IGiftCardCodeService
{
    Task<List<GiftCardCodeDto>> GetAllAsync();
    Task<GiftCardCodeDto?> GetByCodeAsync(string code);
    Task<ErrorOr<GiftCardCodeDto>> CreateAsync(GiftCardCodeCreateDto giftCardCodeDto);
    Task<ErrorOr<GiftCardCodeDto>> UpdateAsync(string code, GiftCardCodeUpdateDto giftCardCodeDto);
    Task<ErrorOr<Success>> DeleteAsync(string code);
}
