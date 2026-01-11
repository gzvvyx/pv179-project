using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Order>> GetAllAsync()
    {
        return _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Orderer)
            .Include(order => order.Creator)
            .ToListAsync();
    }

    public Task<List<Order>> GetAllWithUsersAsync()
    {
        return _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Creator)
            .Include(order => order.Orderer)
            .ToListAsync();
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return _dbContext.Orders
            .Include(order => order.Creator)
            .Include(order => order.Orderer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<List<Order>> GetByOrdererIdAsync(string ordererId)
    {
        return _dbContext.Orders
            .Include(order => order.Creator)
            .Include(order => order.Orderer)
            .Where(o => o.OrdererId == ordererId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Order order)
    {
        if (order.Creator is not null)
        {
            _dbContext.Attach(order.Creator);
        }

        if (order.Orderer is not null)
        {
            _dbContext.Attach(order.Orderer);
        }

        await _dbContext.Orders.AddAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {

        if (order.Creator is not null)
        {
            _dbContext.Attach(order.Creator);
        }

        if (order.Orderer is not null)
        {
            _dbContext.Attach(order.Orderer);
        }

        _dbContext.Orders.Update(order);
    }

    public async Task DeleteAsync(Order order)
    {
        _dbContext.Orders.Remove(order);
    }
}
