using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<List<OrderWithUsersDto>> GetAllWithUsersAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<ErrorOr<List<OrderDto>>> GetByOrdererIdAsync(string ordererId);
    Task<ErrorOr<OrderDto>> CreateAsync(OrderCreateDto orderDto);
    Task<ErrorOr<OrderDto>> UpdateAsync(OrderUpdateDto orderDto);
    Task<ErrorOr<Success>> DeleteAsync(int id);
}
