using DAL.Models;

namespace Infra.Repository;

public interface IGiftCardCodeRepository
{
    Task<List<GiftCardCode>> GetAllAsync();
    Task<GiftCardCode?> GetByCodeAsync(string code);
    Task CreateAsync(GiftCardCode giftCardCode);
    Task UpdateAsync(GiftCardCode giftCardCode);
    Task DeleteAsync(GiftCardCode giftCardCode);
}
