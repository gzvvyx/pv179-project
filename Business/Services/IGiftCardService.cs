using Business.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IGiftCardService
{
    Task<List<GiftCardDto>> GetAllAsync();
    Task<GiftCardDto?> GetByIdAsync(int id);
    Task<(IdentityResult Result, GiftCardDto? GiftCard)> CreateAsync(GiftCardCreateDto giftCardDto);
    Task<(IdentityResult Result, GiftCardDto? GiftCard)> UpdateAsync(int id, GiftCardUpdateDto giftCardDto);
    Task<IdentityResult> DeleteAsync(int id);
}
