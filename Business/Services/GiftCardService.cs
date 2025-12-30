using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class GiftCardService : IGiftCardService
{
    private readonly IGiftCardRepository _giftCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly GiftCardMapper _mapper = new();
    public GiftCardService(IGiftCardRepository giftCardRepository, IUserRepository userRepository)
    {
        _giftCardRepository = giftCardRepository;
        _userRepository = userRepository;
    }
    public async Task<List<GiftCardDto>> GetAllAsync()
    {
        var giftCards = await _giftCardRepository.GetAllAsync();
        return _mapper.Map(giftCards);
    }
    public async Task<GiftCardDto?> GetByIdAsync(int id)
    {
        var giftCard = await _giftCardRepository.GetByIdAsync(id);
        return giftCard == null ? null : _mapper.Map(giftCard);
    }

    public async Task<(IdentityResult Result, GiftCardDto? GiftCard)> CreateAsync(GiftCardCreateDto dto)
    {
        var timestamp = DateTime.UtcNow;

        var giftCard = new GiftCard
        {
            Id = default,
            PriceReduction = dto.PriceReduction,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };

        await _giftCardRepository.CreateAsync(giftCard);

        return (IdentityResult.Success, _mapper.Map(giftCard));
    }

    public async Task<(IdentityResult Result, GiftCardDto? GiftCard)> UpdateAsync(int id, GiftCardUpdateDto dto)
    {
        var giftCard = await _giftCardRepository.GetByIdAsync(id);

        if (giftCard is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "GiftCardNotFound",
                Description = $"GiftCard with id '{id}' was not found."
            }), null);
        }

        if (dto.PriceReduction.HasValue && dto.PriceReduction >= 0.0m && giftCard.PriceReduction != dto.PriceReduction)
        {
            giftCard.PriceReduction = dto.PriceReduction.Value;
        }

        if (dto.GiftCardCodes is not null && dto.GiftCardCodes.Any())
        {
            giftCard.Codes ??= new List<GiftCardCode>();

            foreach (var codeDto in dto.GiftCardCodes)
            {
                giftCard.Codes.Add(new GiftCardCode
                {
                    Code = codeDto.Code,
                    Used = codeDto.Used,
                    GiftCardId = giftCard.Id,
                    GiftCard = codeDto.GiftCard
                });
            }
        }

        giftCard.UpdatedAt = DateTime.UtcNow;

        await _giftCardRepository.UpdateAsync(giftCard);

        return (IdentityResult.Success, _mapper.Map(giftCard));
    }

    public async Task<IdentityResult> DeleteAsync(int id)
    {
        var giftCard = await _giftCardRepository.GetByIdAsync(id);

        if (giftCard is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "GiftCardNotFound",
                Description = $"GiftCard with id '{id}' was not found."
            });
        }

        await _giftCardRepository.DeleteAsync(giftCard);

        return IdentityResult.Success;
    }
}
