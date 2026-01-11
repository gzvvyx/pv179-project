using Business.DTOs;
using Business.Services;
using Business.Validators;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using ErrorOr;
using FluentValidation;
using Infra.Repository;
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Business.Tests;

public class OrderServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<OrderCreateDto> _createValidator;
    private readonly IValidator<OrderUpdateDto> _updateValidator;
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _createValidator = new OrderCreateDtoValidator();
        _updateValidator = new OrderUpdateDtoValidator();
        _orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        _userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        _service = new OrderService(_orderRepo.Object, _userRepo.Object, _dbContext, _createValidator, _updateValidator);
    }

    private static User CreateUser(string id) => new User
    {
        Id = id,
        UserName = $"user-{id}"
    };

    private static Order CreateOrder(
        int id,
        string ordererId,
        string creatorId,
        decimal amount,
        OrderStatus status,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;
        var orderer = CreateUser(ordererId);
        var creator = CreateUser(creatorId);
        return new Order
        {
            Id = id,
            OrdererId = ordererId,
            CreatorId = creatorId,
            Amount = amount,
            Status = status,
            Orderer = orderer,
            Creator = creator,
            CreatedAt = createdAt ?? now,
            UpdatedAt = updatedAt ?? now
        };
    }

    [Fact]
    public async Task CreateAsync_ReturnsNotFound_WhenCreatorNotFound()
    {
        // Arrange

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 10m
        };

        _userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_ReturnsNotFound_WhenOrdererNotFound()
    {
        // Arrange

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 15m
        };

        _userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));
        _userRepo.Setup(r => r.GetByIdAsync(dto.OrdererId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        _userRepo.Verify(r => r.GetByIdAsync(dto.OrdererId), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesOrderAndReturnsDto()
    {
        // Arrange

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 25.5m
        };

        _userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));
        _userRepo.Setup(r => r.GetByIdAsync(dto.OrdererId))
            .ReturnsAsync(CreateUser(dto.OrdererId));

        Order? createdOrder = null;
        _orderRepo.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>(o =>
            {
                createdOrder = o;
                var now = DateTime.UtcNow;
                o.CreatedAt = now;
                o.UpdatedAt = now;
            })
            .Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.IsError);
        var orderDto = result.Value;
        Assert.NotNull(createdOrder);

        Assert.Equal(dto.Amount, createdOrder!.Amount);
        Assert.Equal(dto.OrdererId, createdOrder.OrdererId);
        Assert.Equal(dto.CreatorId, createdOrder.CreatorId);
        Assert.Equal(OrderStatus.Pending, createdOrder.Status);
        Assert.Equal(createdOrder.Amount, orderDto!.Amount);
        Assert.Equal(createdOrder.OrdererId, orderDto.Orderer.Id);
        Assert.Equal(createdOrder.CreatorId, orderDto.Creator.Id);
        Assert.Equal(createdOrder.Status, orderDto.Status);

        Assert.True(orderDto.CreatedAt >= before);
        Assert.True(orderDto.UpdatedAt >= orderDto.CreatedAt);

        _userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        _userRepo.Verify(r => r.GetByIdAsync(dto.OrdererId), Times.Once);
        _orderRepo.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {
        // Arrange

        var orders = new List<Order>
        {
            CreateOrder(1, "orderer-1", "creator-1", 10m, OrderStatus.Completed),
            CreateOrder(2, "orderer-2", "creator-2", 20m, OrderStatus.Pending)
        };

        _orderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var list = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Contains(list, o => o.Id == 1 && o.Amount == 10m && o.Status == OrderStatus.Completed);
        Assert.Contains(list, o => o.Id == 2 && o.Amount == 20m && o.Status == OrderStatus.Pending);

        _orderRepo.Verify(r => r.GetAllAsync(), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        _orderRepo.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Order?)null);

        var result = await _service.GetByIdAsync(123);

        Assert.Null(result);
        _orderRepo.Verify(r => r.GetByIdAsync(123), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        var order = CreateOrder(7, "orderer-7", "creator-7", 77m, OrderStatus.Failed);
        _orderRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(order);

        var dto = await _service.GetByIdAsync(7);

        Assert.NotNull(dto);
        Assert.Equal(order.Id, dto!.Id);
        Assert.Equal(order.Amount, dto.Amount);
        Assert.Equal(order.Status, dto.Status);

        _orderRepo.Verify(r => r.GetByIdAsync(7), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenOrderNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        var dto = new OrderUpdateDto { Id = 5, Amount = 100m, Status = OrderStatus.Completed };

        _orderRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Order?)null);

        var result = await _service.UpdateAsync(dto);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _orderRepo.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAmountAndStatus_WhenProvided()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        var existing = CreateOrder(9, "orderer-9", "creator-9", 10m, OrderStatus.Pending, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(-1));
        _orderRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(existing);

        Order? saved = null;
        _orderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Callback<Order>(o => saved = o)
            .Returns(Task.CompletedTask);

        var update = new OrderUpdateDto { Id = 9, Amount = 50m, Status = OrderStatus.Completed };

        var result = await _service.UpdateAsync(update);

        Assert.False(result.IsError);
        Assert.NotNull(saved);
        Assert.Equal(50m, saved!.Amount);
        Assert.Equal(OrderStatus.Completed, saved.Status);

        Assert.Equal(saved.Amount, result.Value.Amount);
        Assert.Equal(saved.Status, result.Value.Status);

        _orderRepo.Verify(r => r.GetByIdAsync(9), Times.Once);
        _orderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenOrderNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        _orderRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((Order?)null);

        var result = await _service.DeleteAsync(3);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _orderRepo.Verify(r => r.GetByIdAsync(3), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_Success_DeletesOrder()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        var existing = CreateOrder(11, "orderer-11", "creator-11", 99m, OrderStatus.Pending);
        _orderRepo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(existing);
        _orderRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(11);

        Assert.False(result.IsError);

        _orderRepo.Verify(r => r.GetByIdAsync(11), Times.Once);
        _orderRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
        _orderRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }
}

