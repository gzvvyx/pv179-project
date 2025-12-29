using DAL.Models;
using DAL.Models.Enums;
using DAL.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class AppDbContext : IdentityDbContext<User>
{
    private readonly ICurrentUserService? _currentUserService;

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<VideoCategory> VideoCategories { get; set; }

    public AppDbContext(DbContextOptions options, ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public override int SaveChanges()
    {
        AppendAuditLogs();
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AppendAuditLogs();
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AppendAuditLogs()
    {
        var userId = _currentUserService?.GetUserId();
        var now = DateTime.UtcNow;
        var audits = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Video video &&
                (entry.State == EntityState.Added ||
                 entry.State == EntityState.Modified ||
                 entry.State == EntityState.Deleted))
            {
                var id = entry.State == EntityState.Added ? 0 : video.Id;
                var action = entry.State switch
                {
                    EntityState.Added => AuditAction.Create,
                    EntityState.Modified => AuditAction.Update,
                    EntityState.Deleted => AuditAction.Delete,
                    _ => throw new InvalidOperationException()
                };

                audits.Add(new AuditLog
                {
                    Id = default,
                    CreatedAt = now,
                    UpdatedAt = now,
                    UserId = userId,
                    Table = nameof(Videos),
                    EntityId = id,
                    Action = action
                });
            }
        }

        AuditLogs.AddRange(audits);
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                    {
                        entry.Entity.CreatedAt = now;
                    }
                    if (entry.Entity.UpdatedAt == default)
                    {
                        entry.Entity.UpdatedAt = now;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
        
        // Configure VideoCategory relationship with unique constraint
        modelBuilder.Entity<VideoCategory>()
            .HasIndex(vc => new { vc.VideoId, vc.CategoryId })
            .IsUnique();
            
        // Ensure only one primary category per video (PostgreSQL syntax)
        modelBuilder.Entity<VideoCategory>()
            .HasIndex(vc => new { vc.VideoId, vc.IsPrimary })
            .HasFilter("\"IsPrimary\" = true")
            .IsUnique();
    }
}
