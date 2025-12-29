using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto Map(Category category);

    public partial List<CategoryDto> Map(List<Category> categories);

    [MapProperty(nameof(VideoCategory.Category), nameof(VideoCategoryDto.Category))]
    public partial VideoCategoryDto Map(VideoCategory videoCategory);

    public List<VideoCategoryDto> MapVideoCategories(ICollection<VideoCategory> videoCategories)
    {
        return videoCategories.Select(Map).ToList();
    }
}
