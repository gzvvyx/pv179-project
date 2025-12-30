using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using ErrorOr;
using FluentValidation;
using Infra.Repository;

namespace Business.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<SubscriptionCreateDto> _createValidator;
    private readonly IValidator<SubscriptionUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly SubscriptionMapper _mapper = new();

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository, 
        IUserRepository userRepository,
        AppDbContext dbContext,
        IValidator<SubscriptionCreateDto> createValidator,
        IValidator<SubscriptionUpdateDto> updateValidator)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<SubscriptionDto>> GetAllAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return _mapper.Map(subscriptions);
    }

    public async Task<List<SubscriptionWithUsersDto>> GetAllWithUsersAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllWithUsersAsync();
        return subscriptions.Select(subscription => new SubscriptionWithUsersDto
        {
            Subscription = _mapper.Map(subscription),
            OrdererName = subscription.Orderer?.UserName ?? "Unknown",
            CreatorName = subscription.Creator?.UserName ?? "Unknown"
        }).ToList();
    }

    public async Task<SubscriptionDto?> GetByIdAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        return subscription == null ? null : _mapper.Map(subscription);
    }

    public async Task<ErrorOr<SubscriptionDto>> CreateAsync(SubscriptionCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var creator = await _userRepository.GetByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return Error.NotFound();
        }

        var orderer = await _userRepository.GetByIdAsync(dto.OrdererId);
        if (orderer is null)
        {
            return Error.NotFound();
        }

        var timestamp = DateTime.UtcNow;

        var subscription = new Subscription
        {
            Id = default,
            OrdererId = dto.OrdererId,
            Orderer = orderer,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Active = dto.Active!.Value,
            Timeframe = dto.Timeframe!.Value,
            SubscribedAt = timestamp,
            LastRenewedAt = timestamp,
            ExpiresAt = timestamp + dto.Timeframe.Value switch
            {
                SubscriptionTimeframe.Month => TimeSpan.FromDays(30),
                SubscriptionTimeframe.HalfYear => TimeSpan.FromDays(182),
                SubscriptionTimeframe.Year => TimeSpan.FromDays(365),
                _ => TimeSpan.FromDays(30)
            },
            
            CreatedAt = default,
            UpdatedAt = default
        };
        
        await _subscriptionRepository.CreateAsync(subscription);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(subscription);
    }

    public async Task<ErrorOr<SubscriptionDto>> UpdateAsync(SubscriptionUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var subscription = await _subscriptionRepository.GetByIdAsync(dto.Id);

        if (subscription is null)
        {
            return Error.NotFound();
        }

        if (dto.Active.HasValue)
        {
            subscription.Active = dto.Active.Value;
        }

        if (dto.Timeframe.HasValue)
        {
            subscription.Timeframe = dto.Timeframe.Value;
        }

        if (dto.LastRenewedAt.HasValue)
        {
            var lastRenewedAt = dto.LastRenewedAt.Value;
            subscription.LastRenewedAt = lastRenewedAt.Kind switch
            {
                DateTimeKind.Utc => lastRenewedAt,
                DateTimeKind.Local => lastRenewedAt.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(lastRenewedAt, DateTimeKind.Utc),
                _ => lastRenewedAt.ToUniversalTime()
            };
        }

        if (dto.ExpiresAt.HasValue)
        {
            var expiresAt = dto.ExpiresAt.Value;
            subscription.ExpiresAt = expiresAt.Kind switch
            {
                DateTimeKind.Utc => expiresAt,
                DateTimeKind.Local => expiresAt.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc),
                _ => expiresAt.ToUniversalTime()
            };
        }

        await _subscriptionRepository.UpdateAsync(subscription);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(subscription);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);

        if (subscription is null)
        {
            return Error.NotFound();
        }

        await _subscriptionRepository.DeleteAsync(subscription);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
}
