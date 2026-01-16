using API.Extensions;
using API.DTOs;
using API.Mappers;
using Business.DTOs;
using Infra.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly ILogger<VideoController> _logger;
    private readonly IVideoService _videoService;
    private readonly VideoRequestMapper _mapper = new();

    public VideoController(ILogger<VideoController> logger, IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    [HttpGet(Name = "GetVideos")]
    public async Task<IEnumerable<VideoDto>> Get([FromQuery] GetVideosRequestDto request)
    {
        var filter = _mapper.ToFilterDto(request);
        return await _videoService.GetByFilterAsync(filter);
    }

    [HttpGet("paged", Name = "GetVideosPaged")]
    public async Task<ActionResult<PagedResultDto<VideoDto>>> GetPaged([FromQuery] GetVideosPagedRequestDto request)
    {
        var filter = _mapper.ToFilterDto(request);
        var result = await _videoService.GetByFilterPagedAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetVideoById")]
    public async Task<ActionResult<VideoDto>> GetById(int id)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video is null)
        {
            return NotFound();
        }

        return Ok(video);
    }

    [HttpPost(Name = "CreateVideo")]
    public async Task<ActionResult<VideoDto>> Create([FromBody] VideoCreateDto dto)
    {
        var result = await _videoService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetVideoById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateVideo")]
    public async Task<ActionResult<VideoDto>> Update(int id, [FromBody] VideoUpdateRequestDto dto)
    {
        byte[]? thumbnailImageBytes = null;
        string? thumbnailImageFileName = null;

        if (!string.IsNullOrWhiteSpace(dto.ThumbnailImageBase64))
        {
            try
            {
                var base64String = dto.ThumbnailImageBase64;
                if (base64String.Contains(","))
                {
                    base64String = base64String.Split(',')[1];
                }
                thumbnailImageBytes = Convert.FromBase64String(base64String);
                thumbnailImageFileName = dto.ThumbnailImageFileName ?? "thumbnail.jpg";
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 image data.");
            }
        }

        var updateDto = _mapper.ToBusinessDto(dto, id);
        
        if (thumbnailImageBytes != null && !string.IsNullOrEmpty(thumbnailImageFileName))
        {
            updateDto.ThumbnailImageBytes = thumbnailImageBytes;
            updateDto.ThumbnailImageFileName = thumbnailImageFileName;
        }

        var result = await _videoService.UpdateAsync(updateDto);

        return result.ToActionResult();
    }

    [HttpDelete("{id:int}", Name = "DeleteVideo")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _videoService.DeleteAsync(id);

        return result.ToActionResult();
    }
}

