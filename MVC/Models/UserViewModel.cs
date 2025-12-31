namespace pv179.Models;

public class UserViewModel
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public decimal? PricePerMonth { get; set; }
}

    public class UserDetailsViewModel
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public decimal? PricePerMonth { get; set; }
    public IReadOnlyList<VideoItemVM> UploadedVideos { get; set; } = Array.Empty<VideoItemVM>();
    public IReadOnlyList<PlaylistItemVM> CreatedPlaylists { get; set; } = Array.Empty<PlaylistItemVM>();
    public IReadOnlyList<CommentItemVM> Comments { get; set; } = Array.Empty<CommentItemVM>();

    public int VideoCount { get; set; }
    public int PlaylistCount { get; set; }
    public int CommentCount { get; set; }
}
