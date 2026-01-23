using Business.DTOs;

namespace MVC.Models;

public class VideoDetailViewModel
{
    public required VideoDto Video { get; set; }
    public required List<CommentDto> Comments { get; set; }
    public int CommentCount => Comments.Count;
    public string? ReturnUrl { get; set; }
    public List<PlaylistDto> UserPlaylists { get; set; } = new();
    public List<int> PlaylistsContainingVideo { get; set; } = new();
}