using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public class BogusGiftCardSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfGiftCards = 20)
    {
        if (await db.GiftCards.AnyAsync())
            return;

        var giftCardFaker = new Faker<GiftCard>()
            .RuleFor(g => g.PriceReduction, f => f.Finance.Amount(10, 500))
            .RuleFor(g => g.ValidFrom, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(g => g.ValidTo, (f, g) => g.ValidFrom.AddDays(f.Random.Int(30, 365)))
            .RuleFor(g => g.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(g => g.UpdatedAt, (f, g) => g.CreatedAt.AddMinutes(f.Random.Int(1, 200)));

        var giftCards = giftCardFaker.Generate(numberOfGiftCards);

        db.GiftCards.AddRange(giftCards);
        await db.SaveChangesAsync();
    }
}
