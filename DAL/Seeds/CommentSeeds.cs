using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class CommentSeeds
{
    public static readonly Comment CommentCycling = new()
    {
        Id = 1,
        AuthorId = UserSeeds.UserMiaChen.Id,
        Content = "Great video!",
        VideoId = VideoSeeds.VideoAlexUrbanCycling.Id,
        Author = UserSeeds.UserMiaChen,
        Video = VideoSeeds.VideoAlexUrbanCycling,
        CreatedAt = new DateTime(2024, 1, 6, 8, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 1, 8, 1, 45, 0, DateTimeKind.Utc)
    };

    public static readonly Comment CommentDroneExplorationParent = new()
    {
        Id = 2,
        AuthorId = UserSeeds.UserNoahSingh.Id,
        Content = "Is it legal??!",
        VideoId = VideoSeeds.VideoLukaDroneExploration.Id,
        Author = UserSeeds.UserNoahSingh,
        Video = VideoSeeds.VideoLukaDroneExploration,
        Replies = new List<Comment> { CommentDroneExplorationChild },
        CreatedAt = new DateTime(2023, 12, 18, 10, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 12, 24, 9, 45, 32, DateTimeKind.Utc)
    };

    public static readonly Comment CommentDroneExplorationChild = new()
    {
        Id = 3,
        AuthorId = UserSeeds.UserSaraIbrahim.Id,
        Content = "Yes, it is :)",
        VideoId = VideoSeeds.VideoLukaDroneExploration.Id,
        Author = UserSeeds.UserSaraIbrahim,
        Video = VideoSeeds.VideoLukaDroneExploration,
        ParentCommentId = CommentDroneExplorationParent.Id,
        ParentComment = CommentDroneExplorationParent,
        CreatedAt = new DateTime(2023, 12, 19, 10, 0, 20, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2023, 12, 20, 9, 45, 20, DateTimeKind.Utc)
    };

    public static readonly Comment CommentYogaFlow = new()
    {
        Id = 4,
        AuthorId = UserSeeds.UserLukaNovak.Id,
        Content = "Flows like a river",
        VideoId = VideoSeeds.VideoSaraYogaFlow.Id,
        Author = UserSeeds.UserLukaNovak,
        Video = VideoSeeds.VideoSaraYogaFlow,
        CreatedAt = new DateTime(2024, 2, 8, 6, 05, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 2, 8, 7, 0, 0, DateTimeKind.Utc)
    };

    public static readonly Comment CommentDanceWorkshopParent = new()
    {
        Id = 5,
        AuthorId = UserSeeds.UserAlexGomez.Id,
        Content = "Where can I buy the tickets, please?",
        VideoId = VideoSeeds.VideoNoahDanceWorkshop.Id,
        Author = UserSeeds.UserAlexGomez,
        Video = VideoSeeds.VideoNoahDanceWorkshop,
        CreatedAt = new DateTime(2024, 4, 1, 19, 20, 50, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 5, 3, 19, 5, 0, DateTimeKind.Utc)
    };

    public static readonly Comment CommentDanceWorkshopChild = new()
    {
        Id = 6,
        AuthorId = UserSeeds.UserMiaChen.Id,
        Content = "I want to know too!",
        VideoId = VideoSeeds.VideoNoahDanceWorkshop.Id,
        Author = UserSeeds.UserMiaChen,
        Video = VideoSeeds.VideoNoahDanceWorkshop,
        ParentCommentId = CommentDanceWorkshopParent.Id,
        ParentComment = CommentDanceWorkshopParent,
        CreatedAt = new DateTime(2024, 4, 2, 8, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 4, 2, 9, 0, 0, DateTimeKind.Utc)
    };
    public static void LoadLists()
    {
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Comment>().HasData(
            CreateSeed(CommentCycling),
            CreateSeed(CommentDroneExplorationParent),
            CreateSeed(CommentDroneExplorationChild),
            CreateSeed(CommentYogaFlow),
            CreateSeed(CommentDanceWorkshopParent),
            CreateSeed(CommentDanceWorkshopChild)
        );
    }

    private static object CreateSeed(Comment comment)
    {
        return new
        {
            comment.Id,
            comment.AuthorId,
            comment.Content,
            comment.VideoId,
            comment.ParentCommentId,
            comment.ParentComment,
            comment.Replies,
            comment.Author,
            comment.Video,
            comment.CreatedAt,
            comment.UpdatedAt
        };
    }
}
