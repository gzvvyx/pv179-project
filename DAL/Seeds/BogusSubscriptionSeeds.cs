using Bogus;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusSubscriptionSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfSubscriptions = 50)
    {
        if (await db.Subscriptions.AnyAsync())
            return;

        var users = await db.Users.ToListAsync();

        if (users.Count < 2)
            throw new InvalidOperationException("Need at least 2 users to create Subscriptions.");

        var faker = new Faker<Subscription>()
            .RuleFor(s => s.OrdererId, f => f.PickRandom(users).Id)
            .RuleFor(s => s.CreatorId, (f, s) =>
            {
                return f.PickRandom(users.Where(u => u.Id != s.OrdererId)).Id;
            })
            .RuleFor(s => s.Timeframe, f => f.PickRandom<SubscriptionTimeframe>())
            .RuleFor(s => s.SubscribedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(s => s.LastRenewedAt, (f, s) => s.SubscribedAt.AddDays(f.Random.Int(0, 30)))
            .RuleFor(s => s.ExpiresAt, (f, s) =>
            {
                return s.Timeframe switch
                {
                    SubscriptionTimeframe.Month => s.LastRenewedAt.AddMonths(1),
                    SubscriptionTimeframe.HalfYear => s.LastRenewedAt.AddMonths(6),
                    SubscriptionTimeframe.Year => s.LastRenewedAt.AddYears(1),
                    _ => s.LastRenewedAt.AddMonths(1)
                };
            })
            .RuleFor(s => s.Active, (f, s) => s.ExpiresAt > DateTime.UtcNow)
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(s => s.UpdatedAt, (f, s) => s.CreatedAt.AddMinutes(f.Random.Int(1, 500)));

        var subscriptions = faker.Generate(numberOfSubscriptions);

        db.Subscriptions.AddRange(subscriptions);
        await db.SaveChangesAsync();
    }
}
