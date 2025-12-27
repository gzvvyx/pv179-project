using Business.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<List<OrderDto>> GetByOrdererIdAsync(string ordererId);
    Task<(IdentityResult Result, OrderDto? Order)> CreateAsync(OrderCreateDto orderDto);
    Task<(IdentityResult Result, OrderDto? Order)> UpdateAsync(int id, OrderUpdateDto orderDto);
    Task<IdentityResult> DeleteAsync(int id);
}
