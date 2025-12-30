using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace DAL.Seeds;

public class BogusGiftCardCodeSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfCodes = 50)
    {
        if (await db.GiftCardCodes.AnyAsync())
            return;

        var giftCards = await db.GiftCards.ToListAsync();

        if (!giftCards.Any())
            throw new InvalidOperationException("Cannot seed GiftCardCodes before GiftCards exist.");

        var orders = await db.Orders.ToListAsync();

        if (!orders.Any())
            throw new InvalidOperationException("Cannot seed GiftCardCodes before Orders exist.");

        var codeFaker = new Bogus.Faker<GiftCardCode>()
            .RuleFor(gcc => gcc.Code, f => f.Random.AlphaNumeric(10).ToUpper())
            .RuleFor(gcc => gcc.Used, f => f.Random.Bool(0.3f))
            .RuleFor(gcc => gcc.GiftCard, f => f.PickRandom(giftCards))
            .RuleFor(gcc => gcc.Order, (f, gcc) => gcc.Used ? f.PickRandom(orders) : null)
            .RuleFor(gcc => gcc.OrderId, (f, gcc) => gcc.Order?.Id);

        var giftCardCodes = codeFaker.Generate(numberOfCodes);

        db.GiftCardCodes.AddRange(giftCardCodes);
        await db.SaveChangesAsync();
    }
}
