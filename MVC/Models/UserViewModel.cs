using Business.DTOs;
using Infra.DTOs;

namespace pv179.Models;

public class UserViewModel
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public decimal? PricePerMonth { get; set; }
    public bool IsCurrentUser { get; set; }
    public bool IsSubscribed { get; set; }

    public PagedResultDto<VideoDto> Videos { get; set; } = new() { Items = new(), TotalCount = 0, PageNumber = 1, PageSize = 12 };
    public PagedResultDto<PlaylistDto> Playlists { get; set; } = new() { Items = new(), TotalCount = 0, PageNumber = 1, PageSize = 12 };

    public bool HasMore(PagedResultDto<VideoDto> result) => result.PageNumber < (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
    public bool HasMore(PagedResultDto<PlaylistDto> result) => result.PageNumber < (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
}

public class UserEditViewModel
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public decimal? PricePerMonth { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
