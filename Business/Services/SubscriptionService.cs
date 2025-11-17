using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using DAL.Models.Enums;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly SubscriptionMapper _mapper = new();

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async Task<List<SubscriptionDto>> GetAllAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return _mapper.Map(subscriptions);
    }

    public async Task<SubscriptionDto?> GetByIdAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        return subscription == null ? null : _mapper.Map(subscription);
    }

    public async Task<(IdentityResult Result, SubscriptionDto? Subscription)> CreateAsync(SubscriptionCreateDto dto)
    {
        var creator = await _userRepository.GetUserByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "CreatorNotFound",
                Description = $"Creator with id '{dto.CreatorId}' was not found."
            }), null);
        }

        var orderer = await _userRepository.GetUserByIdAsync(dto.OrdererId);
        if (orderer is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "OrdererNotFound",
                Description = $"Orderer with id '{dto.OrdererId}' was not found."
            }), null);
        }

        var timestamp = DateTime.UtcNow;

        var subscription = new Subscription
        {
            Id = default,
            OrdererId = dto.OrdererId,
            Orderer = orderer,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Active = dto.Active,
            Timeframe = dto.Timeframe,

            SubscribedAt = timestamp,
            LastRenewedAt = timestamp,
            ExpiresAt = timestamp + dto.Timeframe switch
            {
                SubscriptionTimeframe.Month => TimeSpan.FromDays(30),
                SubscriptionTimeframe.HalfYear => TimeSpan.FromDays(182),
                SubscriptionTimeframe.Year => TimeSpan.FromDays(365),
                _ => TimeSpan.FromDays(30)
            },
            
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
        
        await _subscriptionRepository.CreateAsync(subscription);

        return (IdentityResult.Success, _mapper.Map(subscription));
    }

    public async Task<(IdentityResult Result, SubscriptionDto? Subscription)> UpdateAsync(int id, SubscriptionUpdateDto dto)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);

        if (subscription is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "SubscriptionNotFound",
                Description = $"Subscription with id '{id}' was not found."
            }), null);
        }

        if (dto.Active.HasValue && subscription.Active != dto.Active)
        {
            subscription.Active = dto.Active.Value;
        }

        if (dto.Timeframe.HasValue && subscription.Timeframe != dto.Timeframe)
        {
            subscription.Timeframe = dto.Timeframe.Value;
        }

        subscription.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);

        return (IdentityResult.Success, _mapper.Map(subscription));
    }

    public async Task<IdentityResult> DeleteAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);

        if (subscription is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "SubscriptionNotFound",
                Description = $"Subscription with id '{id}' was not found."
            });
        }

        await _subscriptionRepository.DeleteAsync(subscription);

        return IdentityResult.Success;
    }
}
