using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class GiftCardCodeService : IGiftCardCodeService
{
    private readonly IGiftCardCodeRepository _giftCardCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly GiftCardCodeMapper _mapper = new();
    public GiftCardCodeService(IGiftCardCodeRepository giftCardCodeRepository, IUserRepository userRepository)
    {
        _giftCardCodeRepository = giftCardCodeRepository;
        _userRepository = userRepository;
    }
    public async Task<List<GiftCardCodeDto>> GetAllAsync()
    {
        var giftCardCodes = await _giftCardCodeRepository.GetAllAsync();
        return _mapper.Map(giftCardCodes);
    }
    public async Task<GiftCardCodeDto?> GetByCodeAsync(string code)
    {
        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);
        return giftCardCode == null ? null : _mapper.Map(giftCardCode);
    }

    public async Task<(IdentityResult Result, GiftCardCodeDto? GiftCardCode)> CreateAsync(GiftCardCodeCreateDto dto)
    {
        var timestamp = DateTime.UtcNow;

        var giftCardCode = new GiftCardCode
        {
            Code = dto.Code,
            Used = dto.Used,
            GiftCardId = dto.GiftCardId,
            GiftCard = dto.GiftCard
        };

        await _giftCardCodeRepository.CreateAsync(giftCardCode);

        return (IdentityResult.Success, _mapper.Map(giftCardCode));
    }

    public async Task<(IdentityResult Result, GiftCardCodeDto? GiftCardCode)> UpdateAsync(string code, GiftCardCodeUpdateDto dto)
    {
        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);

        if (giftCardCode is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "GiftCardCodeNotFound",
                Description = $"GiftCardCode with code '{code}' was not found."
            }), null);
        }

        giftCardCode.Used = dto.Used;
        giftCardCode.OrderId = dto.Used ? dto.OrderId : null;

        await _giftCardCodeRepository.UpdateAsync(giftCardCode);

        return (IdentityResult.Success, _mapper.Map(giftCardCode));
    }

    public async Task<IdentityResult> DeleteAsync(string code)
    {
        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);

        if (giftCardCode is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "GiftCardCodeNotFound",
                Description = $"GiftCardCode with id '{code}' was not found."
            });
        }

        await _giftCardCodeRepository.DeleteAsync(giftCardCode);

        return IdentityResult.Success;
    }
}
