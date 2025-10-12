using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    private readonly ILogger<VideoController> _logger;
    private readonly IVideoService _videoService;

    public VideoController(ILogger<VideoController> logger, IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    [HttpGet(Name = "GetVideos")]
    public async Task<IEnumerable<VideoDto>> Get()
    {
        return await _videoService.GetAllAsync();
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

    [HttpPost]
    public async Task<ActionResult<VideoDto>> Create([FromBody] VideoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, video) = await _videoService.CreateAsync(dto);

        if (!result.Succeeded || video is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetVideoById", new { id = video.Id }, video);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VideoDto>> Update(int id, [FromBody] VideoUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, video) = await _videoService.UpdateAsync(id, dto);

        if (result.Succeeded && video is not null)
        {
            return Ok(video);
        }

        if (result.Errors.Any(error => error.Code == "VideoNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _videoService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "VideoNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}

