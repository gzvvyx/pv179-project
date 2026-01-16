using DAL.Data;
using DAL.Models;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class GiftCardRepository : IGiftCardRepository
{
    private const string GiftCardsGetAllCacheKey = "GiftCards_GetAll";
    private const string GiftCardByIdCacheKeyPrefix = "GiftCard_";
    
    private readonly AppDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GiftCardRepository(AppDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<List<GiftCard>> GetAllAsync()
    {
        return await _cacheService.GetOrSetAsync(GiftCardsGetAllCacheKey, async () =>
        {
            return await _dbContext.GiftCards
                .Include(g => g.Codes)
                .AsNoTracking()
                .ToListAsync();
        });
    }

    public async Task<GiftCard?> GetByIdAsync(int id)
    {
        var cacheKey = $"{GiftCardByIdCacheKeyPrefix}{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _dbContext.GiftCards
                .Include(g => g.Codes)
                .FirstOrDefaultAsync(g => g.Id == id);
        });
    }

    public async Task CreateAsync(GiftCard giftCard)
    {
        await _dbContext.GiftCards.AddAsync(giftCard);
        _cacheService.Remove(GiftCardsGetAllCacheKey);
    }

    public async Task UpdateAsync(GiftCard giftCard)
    {
        _dbContext.GiftCards.Update(giftCard);
        _cacheService.Remove(GiftCardsGetAllCacheKey);
        _cacheService.Remove($"{GiftCardByIdCacheKeyPrefix}{giftCard.Id}");
    }

    public async Task DeleteAsync(GiftCard giftCard)
    {
        _dbContext.GiftCards.Remove(giftCard);
        _cacheService.Remove(GiftCardsGetAllCacheKey);
        _cacheService.Remove($"{GiftCardByIdCacheKeyPrefix}{giftCard.Id}");
    }
}
