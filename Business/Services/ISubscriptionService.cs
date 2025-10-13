using Business.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionDto>> GetAllAsync();
    Task<SubscriptionDto?> GetByIdAsync(int id);
    Task<(IdentityResult Result, SubscriptionDto? Subscription)> CreateAsync(SubscriptionCreateDto subscriptionDto);
    Task<(IdentityResult Result, SubscriptionDto? Subscription)> UpdateAsync(int id, SubscriptionUpdateDto subscriptionDto);
    Task<IdentityResult> DeleteAsync(int id);
}
