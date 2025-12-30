using DAL.Models;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _dbContext;

    public SubscriptionRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public Task<List<Subscription>> GetAllAsync()
    {
        return _dbContext.Subscriptions
            .AsNoTracking()
            .Include(s => s.Orderer)
            .Include(s => s.Creator)
            .ToListAsync();
    }

    public Task<Subscription?> GetByIdAsync(int id)
    {
        return _dbContext.Subscriptions
            .Include(s => s.Creator)
            .Include(s => s.Orderer)
            .FirstOrDefaultAsync(s => s.Id == id);
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
        await _dbContext.SaveChangesAsync();
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
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Subscription subscription)
    {
        _dbContext.Subscriptions.Remove(subscription);
        await _dbContext.SaveChangesAsync();
    }
}
