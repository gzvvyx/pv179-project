using DAL.Models;

namespace Infra.Repository;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetAllWithUsersAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByOrdererIdAsync(string ordererId);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
}
