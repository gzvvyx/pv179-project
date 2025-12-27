using Business.DTOs;
using Business.Services;
using Infra.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistController : ControllerBase
{
    private readonly ILogger<PlaylistController> _logger;
    private readonly IPlaylistService _playlistService;

    public PlaylistController(ILogger<PlaylistController> logger, IPlaylistService playlistService)
    {
        _logger = logger;
        _playlistService = playlistService;
    }

    [HttpGet(Name = "GetPlaylists")]
    public async Task<IEnumerable<PlaylistDto>> Get(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] string? creatorId
    )
    {
        var filter = new PlaylistFilterDto
        {
            Name = name,
            Description = description,
            CreatorId = creatorId
        };

        return await _playlistService.GetByFilterAsync(filter);
    }

    [HttpGet("{id:int}", Name = "GetPlaylistById")]
    public async Task<ActionResult<PlaylistDto>> GetById(int id)
    {
        var playlist = await _playlistService.GetByIdAsync(id);

        if (playlist is null)
        {
            return NotFound();
        }

        return Ok(playlist);
    }

    [HttpPost(Name = "CreatePlaylist")]
    public async Task<ActionResult<PlaylistDto>> Create([FromBody] PlaylistCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, playlist) = await _playlistService.CreateAsync(dto);

        if (!result.Succeeded || playlist is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetPlaylistById", new { id = playlist.Id }, playlist);
    }

    [HttpPut("{id:int}", Name = "UpdatePlaylist")]
    public async Task<ActionResult<PlaylistDto>> Update(int id, [FromBody] PlaylistUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, playlist) = await _playlistService.UpdateAsync(id, dto);

        if (result.Succeeded || playlist is not null)
        {
            return Ok(playlist);
        }

        if (result.Errors.Any(error => error.Code == "PlaylistNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}", Name = "DeletePlaylist")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _playlistService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "PlaylistNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}
