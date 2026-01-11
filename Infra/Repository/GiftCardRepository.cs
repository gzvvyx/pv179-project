using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class GiftCardRepository : IGiftCardRepository
{
    private readonly AppDbContext _dbContext;

    public GiftCardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<GiftCard>> GetAllAsync()
    {
        return _dbContext.GiftCards
            .Include(g => g.Codes)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<GiftCard?> GetByIdAsync(int id)
    {
        return _dbContext.GiftCards
            .Include(g => g.Codes)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task CreateAsync(GiftCard giftCard)
    {
        await _dbContext.GiftCards.AddAsync(giftCard);
    }

    public async Task UpdateAsync(GiftCard giftCard)
    {
        _dbContext.GiftCards.Update(giftCard);
    }

    public async Task DeleteAsync(GiftCard giftCard)
    {
        _dbContext.GiftCards.Remove(giftCard);
    }
}
