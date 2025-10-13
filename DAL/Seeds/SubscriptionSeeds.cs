using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class SubscriptionSeeds
{
    public static readonly Subscription SubscriptionMiaChen = new()
    {
        Id = 1,
        OrdererId = UserSeeds.UserLukaNovak.Id,
        CreatorId = UserSeeds.UserMiaChen.Id,
        Active = true,
        Timeframe = SubscriptionTimeframe.Month,
        SubscribedAt = new DateTime(2023, 1, 15, 10, 0, 0, DateTimeKind.Utc),
        LastRenewedAt = new DateTime(2023, 1, 15, 10, 0, 0, DateTimeKind.Utc).AddMonths(7),
        ExpiresAt = new DateTime(2023, 1, 15, 10, 0, 0, DateTimeKind.Utc).AddMonths(8),
        CreatedAt = new DateTime(2023, 1, 15, 10, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 1, 15, 10, 0, 0, DateTimeKind.Utc).AddMonths(7),
        Orderer = UserSeeds.UserLukaNovak,
        Creator = UserSeeds.UserMiaChen
    };

    public static readonly Subscription SubscriptionSaraIbrahim = new()
    {
        Id = 2,
        OrdererId = UserSeeds.UserAlexGomez.Id,
        CreatorId = UserSeeds.UserSaraIbrahim.Id,
        Active = false,
        Timeframe = SubscriptionTimeframe.HalfYear,
        SubscribedAt = new DateTime(2023, 2, 20, 14, 30, 0, DateTimeKind.Utc),
        LastRenewedAt = new DateTime(2023, 2, 20, 14, 30, 0, DateTimeKind.Utc),
        ExpiresAt = new DateTime(2023, 2, 20, 14, 30, 0, DateTimeKind.Utc).AddDays(182),
        CreatedAt = new DateTime(2023, 2, 20, 14, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 2, 20, 14, 30, 0, DateTimeKind.Utc),
        Orderer = UserSeeds.UserAlexGomez,
        Creator = UserSeeds.UserSaraIbrahim
    };

    public static readonly Subscription SubscriptionAlexGomez = new()
    {
        Id = 3,
        OrdererId = UserSeeds.UserMiaChen.Id,
        CreatorId = UserSeeds.UserAlexGomez.Id,
        Active = false,
        Timeframe = SubscriptionTimeframe.Year,
        SubscribedAt = new DateTime(2022, 3, 10, 9, 15, 0, DateTimeKind.Utc),
        LastRenewedAt = new DateTime(2022, 3, 10, 9, 15, 0, DateTimeKind.Utc).AddYears(1),
        ExpiresAt = new DateTime(2022, 3, 10, 9, 15, 0, DateTimeKind.Utc).AddYears(2),
        CreatedAt = new DateTime(2022, 3, 10, 9, 15, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2022, 3, 10, 9, 15, 0, DateTimeKind.Utc).AddYears(1),
        Orderer = UserSeeds.UserMiaChen,
        Creator = UserSeeds.UserAlexGomez
    };

    public static readonly Subscription SubscriptionLukaNovak = new()
    {
        Id = 4,
        OrdererId = UserSeeds.UserSaraIbrahim.Id,
        CreatorId = UserSeeds.UserLukaNovak.Id,
        Active = true,
        Timeframe = SubscriptionTimeframe.Month,
        SubscribedAt = new DateTime(2023, 4, 5, 11, 45, 0, DateTimeKind.Utc),
        LastRenewedAt = new DateTime(2023, 4, 5, 11, 45, 0, DateTimeKind.Utc),
        ExpiresAt = new DateTime(2023, 5, 5, 11, 45, 0, DateTimeKind.Utc).AddDays(30),
        CreatedAt = new DateTime(2023, 4, 5, 11, 45, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 4, 5, 11, 45, 0, DateTimeKind.Utc),
        Orderer = UserSeeds.UserSaraIbrahim,
        Creator = UserSeeds.UserLukaNovak
    };

    public static readonly Subscription SubscriptionEmmaJohnson = new()
    {
        Id = 5,
        OrdererId = UserSeeds.UserLukaNovak.Id,
        CreatorId = UserSeeds.UserMiaChen.Id,
        Active = true,
        Timeframe = SubscriptionTimeframe.HalfYear,
        SubscribedAt = new DateTime(2023, 5, 12, 16, 0, 0, DateTimeKind.Utc),
        LastRenewedAt = new DateTime(2023, 5, 12, 16, 0, 0, DateTimeKind.Utc),
        ExpiresAt = new DateTime(2023, 5, 12, 16, 0, 0, DateTimeKind.Utc).AddDays(182),
        CreatedAt = new DateTime(2023, 5, 12, 16, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 5, 12, 16, 0, 0, DateTimeKind.Utc),
        Orderer = UserSeeds.UserLukaNovak,
        Creator = UserSeeds.UserMiaChen
    };

    public static void LoadLists()
    {
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Subscription>().HasData(
            CreateSeed(SubscriptionMiaChen),
            CreateSeed(SubscriptionSaraIbrahim),
            CreateSeed(SubscriptionAlexGomez),
            CreateSeed(SubscriptionLukaNovak),
            CreateSeed(SubscriptionEmmaJohnson)
        );
    }

    private static object CreateSeed(Subscription subscription) => new
    {
        subscription.Id,
        subscription.OrdererId,
        subscription.CreatorId,
        subscription.Active,
        subscription.Timeframe,
        subscription.SubscribedAt,
        subscription.LastRenewedAt,
        subscription.ExpiresAt,
        subscription.CreatedAt,
        subscription.UpdatedAt
    };
}
