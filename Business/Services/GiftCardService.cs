using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.Repository;

namespace Business.Services;

public class GiftCardService : IGiftCardService
{
    private readonly IGiftCardRepository _giftCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GiftCardCreateDto> _createValidator;
    private readonly IValidator<GiftCardUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly GiftCardMapper _mapper = new();
    
    public GiftCardService(
        IGiftCardRepository giftCardRepository, 
        IUserRepository userRepository,
        AppDbContext dbContext,
        IValidator<GiftCardCreateDto> createValidator,
        IValidator<GiftCardUpdateDto> updateValidator)
    {
        _giftCardRepository = giftCardRepository;
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _dbContext = dbContext;
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

    public async Task<ErrorOr<GiftCardDto>> CreateAsync(GiftCardCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var giftCard = new GiftCard
        {
            Id = default,
            PriceReduction = dto.PriceReduction,
            ValidFrom = dto.ValidFrom.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.ValidFrom, DateTimeKind.Utc) 
                : dto.ValidFrom.ToUniversalTime(),
            ValidTo = dto.ValidTo.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.ValidTo, DateTimeKind.Utc) 
                : dto.ValidTo.ToUniversalTime(),
            CreatedAt = default,
            UpdatedAt = default
        };
        
        await _giftCardRepository.CreateAsync(giftCard);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(giftCard);
    }

    public async Task<ErrorOr<GiftCardDto>> UpdateAsync(GiftCardUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var giftCard = await _giftCardRepository.GetByIdAsync(dto.Id);

        if (giftCard is null)
        {
            return Error.NotFound();
        }

        if (dto.PriceReduction.HasValue)
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

        await _giftCardRepository.UpdateAsync(giftCard);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(giftCard);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int id)
    {
        var giftCard = await _giftCardRepository.GetByIdAsync(id);

        if (giftCard is null)
        {
            return Error.NotFound();
        }

        await _giftCardRepository.DeleteAsync(giftCard);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
}
