using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionDto>> GetAllAsync();
    Task<List<SubscriptionWithUsersDto>> GetAllWithUsersAsync();
    Task<SubscriptionDto?> GetByIdAsync(int id);
    Task<ErrorOr<SubscriptionDto>> CreateAsync(SubscriptionCreateDto subscriptionDto);
    Task<ErrorOr<SubscriptionDto>> UpdateAsync(SubscriptionUpdateDto subscriptionDto);
    Task<ErrorOr<Success>> DeleteAsync(int id);
}
