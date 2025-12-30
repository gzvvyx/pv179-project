using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class GiftCardCodeRepository : IGiftCardCodeRepository
{
    private readonly AppDbContext _context;

    public GiftCardCodeRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<GiftCardCode>> GetAllAsync()
    {
        return await _context.GiftCardCodes.ToListAsync();
    }
    public async Task<GiftCardCode?> GetByCodeAsync(string code)
    {
        return await _context.GiftCardCodes.FindAsync(code);
    }
    public async Task CreateAsync(GiftCardCode giftCardCode)
    {
        _context.GiftCardCodes.Add(giftCardCode);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(GiftCardCode giftCardCode)
    {
        _context.GiftCardCodes.Update(giftCardCode);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(GiftCardCode giftCardCode)
    {
        _context.GiftCardCodes.Remove(giftCardCode);
        await _context.SaveChangesAsync();
    }
}
