using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class VideoMapper
{
    private readonly CategoryMapper _categoryMapper = new();

    public partial List<VideoDto> Map(List<Video> videos);

    [MapProperty(nameof(Video.VideoCategories), nameof(VideoDto.Categories))]
    public partial VideoDto Map(Video video);

    private List<VideoCategoryDto> MapVideoCategories(ICollection<VideoCategory> videoCategories)
    {
        return _categoryMapper.MapVideoCategories(videoCategories);
    }
}

