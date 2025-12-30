using Business.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IGiftCardCodeService
{
    Task<List<GiftCardCodeDto>> GetAllAsync();
    Task<GiftCardCodeDto?> GetByCodeAsync(string code);
    Task<(IdentityResult Result, GiftCardCodeDto? GiftCardCode)> CreateAsync(GiftCardCodeCreateDto giftCardCodeDto);
    Task<(IdentityResult Result, GiftCardCodeDto? GiftCardCode)> UpdateAsync(string code, GiftCardCodeUpdateDto giftCardCodeDto);
    Task<IdentityResult> DeleteAsync(string code);
}
