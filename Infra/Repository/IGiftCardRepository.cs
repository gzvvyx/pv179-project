using DAL.Models;

namespace Infra.Repository;

public interface IGiftCardRepository
{
    Task<List<GiftCard>> GetAllAsync();
    Task<GiftCard?> GetByIdAsync(int id);
    Task CreateAsync(GiftCard giftCard);
    Task UpdateAsync(GiftCard giftCard);
    Task DeleteAsync(GiftCard giftCard);
}
