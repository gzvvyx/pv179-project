using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.Repository;

namespace Business.Services;

public class GiftCardCodeService : IGiftCardCodeService
{
    private readonly IGiftCardCodeRepository _giftCardCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GiftCardCodeCreateDto> _createValidator;
    private readonly IValidator<GiftCardCodeUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly GiftCardCodeMapper _mapper = new();
    
    public GiftCardCodeService(
        IGiftCardCodeRepository giftCardCodeRepository, 
        IUserRepository userRepository,
        AppDbContext dbContext,
        IValidator<GiftCardCodeCreateDto> createValidator,
        IValidator<GiftCardCodeUpdateDto> updateValidator)
    {
        _giftCardCodeRepository = giftCardCodeRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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

    public async Task<ErrorOr<GiftCardCodeDto>> CreateAsync(GiftCardCodeCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var giftCardCode = new GiftCardCode
        {
            Code = dto.Code,
            Used = dto.Used,
            GiftCardId = dto.GiftCardId,
            GiftCard = dto.GiftCard
        };

        await _giftCardCodeRepository.CreateAsync(giftCardCode);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(giftCardCode);
    }

    public async Task<ErrorOr<GiftCardCodeDto>> UpdateAsync(string code, GiftCardCodeUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);

        if (giftCardCode is null)
        {
            return Error.NotFound();
        }

        giftCardCode.Used = dto.Used;
        giftCardCode.OrderId = dto.Used ? dto.OrderId : null;

        await _giftCardCodeRepository.UpdateAsync(giftCardCode);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(giftCardCode);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string code)
    {
        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);

        if (giftCardCode is null)
        {
            return Error.NotFound();
        }

        await _giftCardCodeRepository.DeleteAsync(giftCardCode);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
}
