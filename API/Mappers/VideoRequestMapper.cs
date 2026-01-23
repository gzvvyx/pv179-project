using API.DTOs;
using Business.DTOs;
using Infra.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class VideoRequestMapper
{
    [MapperIgnoreTarget(nameof(VideoUpdateDto.ThumbnailImageBytes))]
    [MapperIgnoreSource(nameof(VideoUpdateRequestDto.ThumbnailImageBase64))]
    public partial VideoUpdateDto ToBusinessDto(VideoUpdateRequestDto dto, int id);
    
    [MapProperty(nameof(GetVideosRequestDto.Author), nameof(VideoFilterDto.CreatorId))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.FromDate))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.ToDate))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.SortBy))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.SortDescending))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.PageNumber))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.PageSize))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.CreatorIds))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.CategoryId))]
    [MapperIgnoreTarget(nameof(VideoFilterDto.CategoryIds))]
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
    [MapperIgnoreTarget(nameof(VideoFilterDto.CreatorIds))]
    private partial VideoFilterDto ToFilterDtoCore(GetVideosPagedRequestDto dto);
}

