using API.DTOs;
using Business.DTOs;
using Riok.Mapperly.Abstractions;

namespace API.Mappers;

[Mapper]
public partial class VideoRequestMapper
{
    public partial VideoUpdateDto ToBusinessDto(VideoUpdateRequestDto dto, int id);
}

