using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers;

[Mapper]
public partial class VideoMapper
{
    public partial List<VideoDto> Map(List<Video> videos);
    public partial VideoDto Map(Video video);
}

