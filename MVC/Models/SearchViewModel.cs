using Business.DTOs;
using Infra.DTOs;

namespace pv179.Models;

public class SearchViewModel
{
    public string Query { get; set; } = string.Empty;
    public string Category { get; set; } = "all";
    public List<CategoryDto> AvailableCategories { get; set; } = new();
}

public class SearchResultsViewModel<T>
{
    public string Query { get; set; } = string.Empty;
    public List<T> Results { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

public class SearchAllResultsViewModel
{
    public string Query { get; set; } = string.Empty;
    public PagedResultDto<VideoDto> Videos { get; set; } = new() { Items = new(), TotalCount = 0, PageNumber = 1, PageSize = 12 };
    public PagedResultDto<PlaylistDto> Playlists { get; set; } = new() { Items = new(), TotalCount = 0, PageNumber = 1, PageSize = 12 };
    public PagedResultDto<UserDto> Creators { get; set; } = new() { Items = new(), TotalCount = 0, PageNumber = 1, PageSize = 12 };
    public List<CategoryDto> AvailableCategories { get; set; } = new();
    public List<int> SelectedCategoryIds { get; set; } = new();
}
