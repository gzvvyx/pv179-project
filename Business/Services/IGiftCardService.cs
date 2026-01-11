using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface IGiftCardService
{
    Task<List<GiftCardDto>> GetAllAsync();
    Task<GiftCardDto?> GetByIdAsync(int id);
    Task<ErrorOr<GiftCardDto>> CreateAsync(GiftCardCreateDto giftCardDto);
    Task<ErrorOr<GiftCardDto>> UpdateAsync(GiftCardUpdateDto giftCardDto);
    Task<ErrorOr<Success>> DeleteAsync(int id);
}
