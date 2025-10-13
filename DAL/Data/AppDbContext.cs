using DAL.Models;
using DAL.Seeds;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class AppDbContext : IdentityDbContext
{
    private readonly bool _seedData;

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public AppDbContext(DbContextOptions options, bool seedData = false) : base(options)
    {
        _seedData = seedData;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
            .HasOne(o => o.Orderer)
            .WithMany(u => u.OrdersPlaced)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>()
            .HasOne(o => o.Creator)
            .WithMany(u => u.OrdersReceived)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Subscription>()
            .HasOne(s => s.Orderer)
            .WithMany(u => u.SubscriptionsPurchased)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Subscription>()
            .HasOne(s => s.Creator)
            .WithMany(u => u.SubscriptionsOffered)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Video>()
            .HasOne(v => v.Creator)
            .WithMany(u => u.Videos)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Playlist>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.Playlists)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.Video)
            .WithMany(v => v.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .OnDelete(DeleteBehavior.Restrict);

        if (!_seedData) return;
        SeedsInit.LoadLists();

        UserSeeds.Seed(builder);
        OrderSeeds.Seed(builder);
        SubscriptionSeeds.Seed(builder);
        VideoSeeds.Seed(builder);
        PlaylistSeeds.Seed(builder);
        CommentSeeds.Seed(builder);
    }
}
