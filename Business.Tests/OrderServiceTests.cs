using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DTOs;
using Business.Services;
using DAL.Models;
using DAL.Models.Enums;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Business.Tests;

public class OrderServiceTests
{
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
    public async Task CreateAsync_ReturnsError_WhenCreatorNotFound()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 10m
        };

        userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync((User?)null);

        // Act
        var (result, order) = await service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(order);
        var error = Assert.Single(result.Errors);
        Assert.Equal("CreatorNotFound", error.Code);

        userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        userRepo.VerifyNoOtherCalls();
        orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenOrdererNotFound()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 15m
        };

        userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));
        userRepo.Setup(r => r.GetByIdAsync(dto.OrdererId))
            .ReturnsAsync((User?)null);

        // Act
        var (result, order) = await service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(order);
        var error = Assert.Single(result.Errors);
        Assert.Equal("OrdererNotFound", error.Code);

        userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        userRepo.Verify(r => r.GetByIdAsync(dto.OrdererId), Times.Once);
        userRepo.VerifyNoOtherCalls();
        orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesOrderAndReturnsDto()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var dto = new OrderCreateDto
        {
            CreatorId = "creator-1",
            OrdererId = "orderer-1",
            Amount = 25.5m
        };

        userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));
        userRepo.Setup(r => r.GetByIdAsync(dto.OrdererId))
            .ReturnsAsync(CreateUser(dto.OrdererId));

        Order? createdOrder = null;
        orderRepo.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>(o => createdOrder = o)
            .Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        // Act
        var (result, orderDto) = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(orderDto);
        Assert.NotNull(createdOrder);

        Assert.Equal(dto.Amount, createdOrder!.Amount);
        Assert.Equal(dto.OrdererId, createdOrder.OrdererId);
        Assert.Equal(dto.CreatorId, createdOrder.CreatorId);
        Assert.Equal(OrderStatus.Pending, createdOrder.Status);

        Assert.Equal(createdOrder.Amount, orderDto!.Amount);
        Assert.Equal(createdOrder.OrdererId, orderDto.OrdererId);
        Assert.Equal(createdOrder.CreatorId, orderDto.CreatorId);
        Assert.Equal(createdOrder.Status, orderDto.Status);

        Assert.True(orderDto.CreatedAt >= before);
        Assert.True(orderDto.UpdatedAt >= orderDto.CreatedAt);

        userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        userRepo.Verify(r => r.GetByIdAsync(dto.OrdererId), Times.Once);
        orderRepo.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        userRepo.VerifyNoOtherCalls();
        orderRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var orders = new List<Order>
        {
            CreateOrder(1, "orderer-1", "creator-1", 10m, OrderStatus.Completed),
            CreateOrder(2, "orderer-2", "creator-2", 20m, OrderStatus.Pending)
        };

        orderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var list = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Contains(list, o => o.Id == 1 && o.Amount == 10m && o.Status == OrderStatus.Completed);
        Assert.Contains(list, o => o.Id == 2 && o.Amount == 20m && o.Status == OrderStatus.Pending);

        orderRepo.Verify(r => r.GetAllAsync(), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        orderRepo.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Order?)null);

        var result = await service.GetByIdAsync(123);

        Assert.Null(result);
        orderRepo.Verify(r => r.GetByIdAsync(123), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var order = CreateOrder(7, "orderer-7", "creator-7", 77m, OrderStatus.Failed);
        orderRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(order);

        var dto = await service.GetByIdAsync(7);

        Assert.NotNull(dto);
        Assert.Equal(order.Id, dto!.Id);
        Assert.Equal(order.Amount, dto.Amount);
        Assert.Equal(order.Status, dto.Status);

        orderRepo.Verify(r => r.GetByIdAsync(7), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenOrderNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        orderRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Order?)null);

        var (result, updated) = await service.UpdateAsync(5, new OrderUpdateDto { Amount = 100m, Status = OrderStatus.Completed });

        Assert.False(result.Succeeded);
        Assert.Null(updated);
        var error = Assert.Single(result.Errors);
        Assert.Equal("OrderNotFound", error.Code);

        orderRepo.Verify(r => r.GetByIdAsync(5), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAmountAndStatus_WhenProvided()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var existing = CreateOrder(9, "orderer-9", "creator-9", 10m, OrderStatus.Pending, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(-1));
        orderRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(existing);

        Order? saved = null;
        orderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Callback<Order>(o => saved = o)
            .Returns(Task.CompletedTask);

        var update = new OrderUpdateDto { Amount = 50m, Status = OrderStatus.Completed };

        var (result, dto) = await service.UpdateAsync(9, update);

        Assert.True(result.Succeeded);
        Assert.NotNull(dto);
        Assert.NotNull(saved);
        Assert.Equal(50m, saved!.Amount);
        Assert.Equal(OrderStatus.Completed, saved.Status);
        Assert.True(saved.UpdatedAt >= saved.CreatedAt);

        Assert.Equal(saved.Amount, dto!.Amount);
        Assert.Equal(saved.Status, dto.Status);

        orderRepo.Verify(r => r.GetByIdAsync(9), Times.Once);
        orderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsError_WhenOrderNotFound()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        orderRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((Order?)null);

        var result = await service.DeleteAsync(3);

        Assert.False(result.Succeeded);
        var error = Assert.Single(result.Errors);
        Assert.Equal("OrderNotFound", error.Code);

        orderRepo.Verify(r => r.GetByIdAsync(3), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_Success_DeletesOrder()
    {
        var orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new OrderService(orderRepo.Object, userRepo.Object);

        var existing = CreateOrder(11, "orderer-11", "creator-11", 99m, OrderStatus.Pending);
        orderRepo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(existing);
        orderRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        var result = await service.DeleteAsync(11);

        Assert.True(result.Succeeded);

        orderRepo.Verify(r => r.GetByIdAsync(11), Times.Once);
        orderRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
        orderRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }
}

