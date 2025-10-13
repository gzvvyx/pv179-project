using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class OrderSeeds
{
    public static readonly Order OrderAlexGomezChannel = new()
    {
        Id = 1,
        CreatorId = UserSeeds.UserAlexGomez.Id,
        OrdererId = UserSeeds.UserSaraIbrahim.Id,
        Amount = 100.00m,
        Status = OrderStatus.Completed,
        CreatedAt = new DateTime(2023, 1, 15, 6, 8, 7, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 1, 16, 4, 8, 10, DateTimeKind.Utc),

        Orderer = UserSeeds.UserSaraIbrahim,
        Creator = UserSeeds.UserAlexGomez,
    };

    public static readonly Order OrderMiaChenChannel = new()
    {
        Id = 2,
        CreatorId = UserSeeds.UserMiaChen.Id,
        OrdererId = UserSeeds.UserAlexGomez.Id,
        Amount = 119.99m,
        Status = OrderStatus.Failed,
        CreatedAt = new DateTime(2025, 2, 20, 10, 15, 30, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 2, 20, 10, 15, 30, DateTimeKind.Utc),
        Orderer = UserSeeds.UserAlexGomez,
        Creator = UserSeeds.UserMiaChen,
    };

    public static readonly Order OrderLukaNovakChannel = new()
    {
        Id = 3,
        CreatorId = UserSeeds.UserLukaNovak.Id,
        OrdererId = UserSeeds.UserMiaChen.Id,
        Amount = 75.50m,
        Status = OrderStatus.Completed,
        CreatedAt = new DateTime(2024, 3, 5, 14, 45, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 3, 5, 17, 0, 0, DateTimeKind.Utc),
        Orderer = UserSeeds.UserMiaChen,
        Creator = UserSeeds.UserLukaNovak,
    };

    public static readonly Order OrderSaraIbrahimChannel = new()
    {
        Id = 4,
        CreatorId = UserSeeds.UserSaraIbrahim.Id,
        OrdererId = UserSeeds.UserLukaNovak.Id,
        Amount = 200.00m,
        Status = OrderStatus.Pending,
        CreatedAt = new DateTime(2024, 4, 10, 9, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 4, 10, 9, 30, 0, DateTimeKind.Utc),
        Orderer = UserSeeds.UserLukaNovak,
        Creator = UserSeeds.UserSaraIbrahim,
    };

    public static readonly Order OrderNoahSinghChannel = new()
    {
        Id = 5,
        CreatorId = UserSeeds.UserNoahSingh.Id,
        OrdererId = UserSeeds.UserSaraIbrahim.Id,
        Amount = 49.99m,
        Status = OrderStatus.Failed,
        CreatedAt = new DateTime(2025, 5, 25, 11, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 6, 1, 8, 0, 5, DateTimeKind.Utc),
        Orderer = UserSeeds.UserSaraIbrahim,
        Creator = UserSeeds.UserNoahSingh,
    };

    public static void LoadLists()
    {
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Order>().HasData(
            CreateSeed(OrderAlexGomezChannel),
            CreateSeed(OrderMiaChenChannel),
            CreateSeed(OrderLukaNovakChannel),
            CreateSeed(OrderSaraIbrahimChannel),
            CreateSeed(OrderNoahSinghChannel)
        );
    }

    private static object CreateSeed(Order order) => new
    {
        order.Id,
        order.CreatorId,
        order.OrdererId,
        order.Amount,
        order.Status,
        order.CreatedAt,
        order.UpdatedAt
    };
}
