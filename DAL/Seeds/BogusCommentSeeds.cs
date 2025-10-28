using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusCommentSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfComments = 50, int numberOfReplies = 25)
    {
        if (await db.Comments.AnyAsync())
            return;

        var users = await db.Users.ToListAsync();
        var videos = await db.Videos.ToListAsync();

        if (!users.Any() || !videos.Any())
            throw new InvalidOperationException("Cannot seed Comments before Users and Videos exist.");

        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.AuthorId, f => f.PickRandom(users).Id)
            .RuleFor(c => c.VideoId, f => f.PickRandom(videos).Id)
            .RuleFor(c => c.Content, f => f.Lorem.Sentences(f.Random.Int(1, 3)))
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(c => c.UpdatedAt, (f, c) => c.CreatedAt.AddMinutes(f.Random.Int(1, 200)))
            .RuleFor(c => c.ParentCommentId, f => (int?)null);

        var comments = commentFaker.Generate(numberOfComments);

        db.Comments.AddRange(comments);
        await db.SaveChangesAsync();

        var replyFaker = new Faker<Comment>()
            .RuleFor(c => c.AuthorId, f => f.PickRandom(users).Id)
            .RuleFor(c => c.VideoId, f => f.PickRandom(videos).Id)
            .RuleFor(c => c.Content, f => f.Lorem.Sentences(f.Random.Int(1, 2)))
            .RuleFor(c => c.CreatedAt, f => f.Date.Recent(30).ToUniversalTime())
            .RuleFor(c => c.UpdatedAt, (f, c) => c.CreatedAt.AddMinutes(f.Random.Int(1, 100)))
            .RuleFor(c => c.ParentCommentId, f => f.PickRandom(comments).Id);

        var replies = replyFaker.Generate(numberOfReplies);

        db.Comments.AddRange(replies);

        await db.SaveChangesAsync();
    }
}