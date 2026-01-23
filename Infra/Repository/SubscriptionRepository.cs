using DAL.Models;
using DAL.Data;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{
    private const string SubscriptionsGetAllCacheKey = "Subscriptions_GetAll";
    private const string SubscriptionByIdCacheKeyPrefix = "Subscription_";
    private const string SubscriptionCheckCacheKeyPrefix = "Subscription_Check_";
    
    private readonly AppDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public SubscriptionRepository(AppDbContext context, ICacheService cacheService)
    {
        _dbContext = context;
        _cacheService = cacheService;
    }

    public async Task<List<Subscription>> GetAllAsync()
    {
        return await _cacheService.GetOrSetAsync(SubscriptionsGetAllCacheKey, async () =>
        {
            return await _dbContext.Subscriptions
                .AsNoTracking()
                .Include(s => s.Orderer)
                .Include(s => s.Creator)
                .ToListAsync();
        });
    }

    public Task<List<Subscription>> GetAllWithUsersAsync()
    {
        return _dbContext.Subscriptions
            .AsNoTracking()
            .Include(s => s.Creator)
            .Include(s => s.Orderer)
            .ToListAsync();
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
        var cacheKey = $"{SubscriptionByIdCacheKeyPrefix}{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _dbContext.Subscriptions
                .Include(s => s.Creator)
                .Include(s => s.Orderer)
                .FirstOrDefaultAsync(s => s.Id == id);
        });
    }
    

    public async Task CreateAsync(Subscription subscription)
    {
        if (subscription.Creator is not null)
        {
            _dbContext.Attach(subscription.Creator);
        }

        if (subscription.Orderer is not null)
        {
            _dbContext.Attach(subscription.Orderer);
        }

        await _dbContext.Subscriptions.AddAsync(subscription);
        
        _cacheService.Remove(SubscriptionsGetAllCacheKey);
        if (subscription.OrdererId != null && subscription.CreatorId != null)
        {
            _cacheService.Remove($"{SubscriptionCheckCacheKeyPrefix}{subscription.OrdererId}_{subscription.CreatorId}");
        }
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        if (subscription.Creator is not null)
        {
            _dbContext.Attach(subscription.Creator);
        }

        if (subscription.Orderer is not null)
        {
            _dbContext.Attach(subscription.Orderer);
        }

        _dbContext.Subscriptions.Update(subscription);
        
        _cacheService.Remove(SubscriptionsGetAllCacheKey);
        _cacheService.Remove($"{SubscriptionByIdCacheKeyPrefix}{subscription.Id}");
        if (subscription.OrdererId != null && subscription.CreatorId != null)
        {
            _cacheService.Remove($"{SubscriptionCheckCacheKeyPrefix}{subscription.OrdererId}_{subscription.CreatorId}");
        }
    }

    public async Task DeleteAsync(Subscription subscription)
    {
        _dbContext.Subscriptions.Remove(subscription);
        
        _cacheService.Remove(SubscriptionsGetAllCacheKey);
        _cacheService.Remove($"{SubscriptionByIdCacheKeyPrefix}{subscription.Id}");
        if (subscription.OrdererId != null && subscription.CreatorId != null)
        {
            _cacheService.Remove($"{SubscriptionCheckCacheKeyPrefix}{subscription.OrdererId}_{subscription.CreatorId}");
        }
    }

    public async Task<bool> IsUserSubscribedAsync(User orderer, User creator)
    {
        var now = DateTime.UtcNow;

        var cacheKey = $"{SubscriptionCheckCacheKeyPrefix}{orderer.Id}_{creator.Id}";
        var result = await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var isSubscribed = await _dbContext.Subscriptions
                .AsNoTracking()
                .AnyAsync(s => s.OrdererId == orderer.Id
                            && s.CreatorId == creator.Id
                            && s.Active
                            && s.ExpiresAt > now);

            return new { IsSubscribed = isSubscribed };
        });
        return result.IsSubscribed;
    }

    public Task<List<Subscription>> GetBySubscriberAsync(User subscriber)
    {
        return _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.OrdererId == subscriber.Id)
            .Include(s => s.Creator)
            .Include(s => s.Orderer)
            .ToListAsync();
    }
}
