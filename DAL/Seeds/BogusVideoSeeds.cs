using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.Seeds;

public static class BogusVideoSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfVideos = 50)
    {
        if (await db.Videos.AnyAsync())
            return;

        var users = await db.Users.ToListAsync();

        if (!users.Any())
            throw new InvalidOperationException("Cannot seed Videos before Users exist.");

        var faker = new Faker<Video>()
            .RuleFor(v => v.CreatorId, f => f.PickRandom(users).Id)
            .RuleFor(v => v.Title, f => f.Lorem.Sentence(5))
            .RuleFor(v => v.Description, f => f.Lorem.Paragraph(2))
            .RuleFor(v => v.Url, f => f.Internet.UrlWithPath("https", "videos.example.com", $"video/{f.Random.Guid()}"))
            .RuleFor(v => v.ThumbnailUrl, f => f.Image.PicsumUrl(width: 640, height: 360, blur: f.Random.Bool()))
            .RuleFor(v => v.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(v => v.UpdatedAt, (f, v) => v.CreatedAt.AddMinutes(f.Random.Int(1, 500)));

        var videos = faker.Generate(numberOfVideos);

        db.Videos.AddRange(videos);
        await db.SaveChangesAsync();
    }
}