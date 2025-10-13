using DAL.Models;

namespace Infra.Repository;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
}
