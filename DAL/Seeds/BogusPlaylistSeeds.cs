using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusPlaylistSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfPlaylists = 20)
    {
        if (await db.Playlists.AnyAsync())
            return;

        var users = await db.Users.ToListAsync();
        var videos = await db.Videos.ToListAsync();

        if (!users.Any() || !videos.Any())
            throw new InvalidOperationException("Cannot seed Playlists before Users and Videos exist.");

        var faker = new Faker<Playlist>()
            .RuleFor(p => p.CreatorId, f => f.PickRandom(users).Id)
            .RuleFor(p => p.Name, f => $"{f.Hacker.Adjective()} {f.Music.Genre()} Mix")
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(p => p.UpdatedAt, (f, p) => p.CreatedAt.AddMinutes(f.Random.Int(1, 500)));

        var playlists = faker.Generate(numberOfPlaylists);

        foreach (var playlist in playlists)
        {
            var count = new Random().Next(3, 10);
            var selectedVideos = videos.OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
            foreach (var video in selectedVideos)
            {
                playlist.Videos.Add(video);
            }
        }

        db.Playlists.AddRange(playlists);
        await db.SaveChangesAsync();
    }
}