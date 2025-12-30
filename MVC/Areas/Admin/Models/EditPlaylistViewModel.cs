using Business.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models;

public class EditPlaylistViewModel
{
    public PlaylistDto Playlist { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    
    [StringLength(200)]
    public string? Name { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
}

