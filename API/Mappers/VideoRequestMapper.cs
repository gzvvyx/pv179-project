using API.DTOs;
using Business.DTOs;
using Infra.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class VideoRequestMapper
{
    public partial VideoUpdateDto ToBusinessDto(VideoUpdateRequestDto dto, int id);
    
    [MapProperty(nameof(GetVideosRequestDto.Author), nameof(VideoFilterDto.CreatorId))]
    public partial VideoFilterDto ToFilterDto(GetVideosRequestDto dto);
    
    public VideoFilterDto ToFilterDto(GetVideosPagedRequestDto dto)
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
    
    [MapProperty(nameof(GetVideosPagedRequestDto.Author), nameof(VideoFilterDto.CreatorId))]
    private partial VideoFilterDto ToFilterDtoCore(GetVideosPagedRequestDto dto);
}

