using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusOrderSeeds
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Orders.AnyAsync())
            return;

        var users = await db.Users.ToListAsync();

        if (users.Count < 2)
            throw new InvalidOperationException("Need at least 2 users to create Orders.");

        var faker = new Faker<Order>()
            .RuleFor(o => o.OrdererId, f => f.PickRandom(users).Id)
            .RuleFor(o => o.CreatorId, (f, o) =>
            {
                return f.PickRandom(users.Where(u => u.Id != o.OrdererId)).Id;
            })
            .RuleFor(o => o.Amount, f => f.Finance.Amount(5, 200))
            .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(o => o.UpdatedAt, (f, o) => o.CreatedAt.AddMinutes(f.Random.Int(1, 500)));

        var orders = faker.Generate(50);

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();
    }
}
