using DAL.Models;

namespace Infra.Repository;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAllAsync();
    Task<List<Subscription>> GetAllWithUsersAsync();
    Task<Subscription?> GetByIdAsync(int id);
    Task CreateAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    Task DeleteAsync(Subscription subscription);
}
