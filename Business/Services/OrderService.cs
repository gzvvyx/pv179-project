using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using ErrorOr;
using FluentValidation;
using Infra.Repository;

namespace Business.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<OrderCreateDto> _createValidator;
    private readonly IValidator<OrderUpdateDto> _updateValidator;
    private readonly AppDbContext _dbContext;
    private readonly OrderMapper _mapper = new();
    
    public OrderService(
        IOrderRepository orderRepository, 
        IUserRepository userRepository,
        AppDbContext dbContext,
        IValidator<OrderCreateDto> createValidator,
        IValidator<OrderUpdateDto> updateValidator)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _dbContext = dbContext;
    }
    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map(orders);
    }

    public async Task<List<OrderWithUsersDto>> GetAllWithUsersAsync()
    {
        var orders = await _orderRepository.GetAllWithUsersAsync();
        return orders.Select(order => new OrderWithUsersDto
        {
            Order = _mapper.Map(order),
            OrdererName = order.Orderer?.UserName ?? "Unknown",
            CreatorName = order.Creator?.UserName ?? "Unknown"
        }).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : _mapper.Map(order);
    }

    public async Task<ErrorOr<List<OrderDto>>> GetByOrdererIdAsync(string ordererId)
    {
        var orderer = await _userRepository.GetByIdAsync(ordererId);
        if (orderer is null)
        {
            return Error.NotFound();
        }

        var orders = await _orderRepository.GetByOrdererAsync(orderer);

        return _mapper.Map(orders);
    }

    public async Task<ErrorOr<OrderDto>> CreateAsync(OrderCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var creator = await _userRepository.GetByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return Error.NotFound();
        }

        var orderer = await _userRepository.GetByIdAsync(dto.OrdererId);
        if (orderer is null)
        {
            return Error.NotFound();
        }

        var order = new Order
        {
            Id = default,
            OrdererId = dto.OrdererId,
            CreatorId = dto.CreatorId,
            Amount = dto.Amount!.Value, 
            Status = OrderStatus.Pending,
            Orderer = orderer,
            Creator = creator,
            CreatedAt = default, 
            UpdatedAt = default 
        };
        
        await _orderRepository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(order);
    }

    public async Task<ErrorOr<OrderDto>> UpdateAsync(OrderUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var order = await _orderRepository.GetByIdAsync(dto.Id);

        if (order is null)
        {
            return Error.NotFound();
        }

        if (dto.Amount.HasValue)
        {
            order.Amount = dto.Amount.Value;
        }
        
        if (dto.Status.HasValue)
        {
            order.Status = dto.Status.Value;
        }

        await _orderRepository.UpdateAsync(order);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map(order);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return Error.NotFound();
        }

        await _orderRepository.DeleteAsync(order);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
}
