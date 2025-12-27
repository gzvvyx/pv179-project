using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using DAL.Models.Enums;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly OrderMapper _mapper = new();
    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }
    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : _mapper.Map(order);
    }

    public async Task<List<OrderDto>> GetByOrdererIdAsync(string ordererId)
    {
        var orders = await _orderRepository.GetByOrdererIdAsync(ordererId);
        return _mapper.Map(orders);
    }

    public async Task<(IdentityResult Result, OrderDto? Order)> CreateAsync(OrderCreateDto dto)
    {
        var creator = await _userRepository.GetUserByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "CreatorNotFound",
                Description = $"Creator with id '{dto.CreatorId}' was not found."
            }), null);
        }

        var orderer = await _userRepository.GetUserByIdAsync(dto.OrdererId);
        if (orderer is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "OrdererNotFound",
                Description = $"Orderer with id '{dto.OrdererId}' was not found."
            }), null);
        }

        var timestamp = DateTime.UtcNow;

        var order = new Order
        {
            Id = default,
            OrdererId = dto.OrdererId,
            Orderer = orderer,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Amount = dto.Amount,
            Status = OrderStatus.Pending,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
        
        await _orderRepository.CreateAsync(order);

        return (IdentityResult.Success, _mapper.Map(order));
    }

    public async Task<(IdentityResult Result, OrderDto? Order)> UpdateAsync(int id, OrderUpdateDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "OrderNotFound",
                Description = $"Order with id '{id}' was not found."
            }), null);
        }
        
        if (dto.Amount.HasValue && dto.Amount.Value >= 0.0m && order.Amount != dto.Amount)
        {
            order.Amount = dto.Amount.Value;
        }

        if (dto.Status.HasValue && order.Status != dto.Status)
        {
            order.Status = dto.Status.Value;
        }

        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);

        return (IdentityResult.Success, _mapper.Map(order));
    }

    public async Task<IdentityResult> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "OrderNotFound",
                Description = $"Order with id '{id}' was not found."
            });
        }

        await _orderRepository.DeleteAsync(order);

        return IdentityResult.Success;
    }
}
