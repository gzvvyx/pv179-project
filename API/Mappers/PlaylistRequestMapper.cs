using API.DTOs;
using Infra.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class PlaylistRequestMapper
{
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.PageNumber))]
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.PageSize))]
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.FromDate))]
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.ToDate))]
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.SortBy))]
    [MapperIgnoreTarget(nameof(PlaylistFilterDto.SortDescending))]
    public partial PlaylistFilterDto ToFilterDto(GetPlaylistsRequestDto dto);
    
    public PlaylistFilterDto ToFilterDto(GetPlaylistsPagedRequestDto dto)
    {
        var filter = ToFilterDtoCore(dto);
        
        if (!string.IsNullOrEmpty(dto.FromDate) && DateTime.TryParse(dto.FromDate, out var parsedFromDate))
        {
            filter.FromDate = parsedFromDate;
        }
        if (!string.IsNullOrEmpty(dto.ToDate) && DateTime.TryParse(dto.ToDate, out var parsedToDate))
        {
            filter.ToDate = parsedToDate;
        }
        
        return filter;
    }
    
    private partial PlaylistFilterDto ToFilterDtoCore(GetPlaylistsPagedRequestDto dto);
}
