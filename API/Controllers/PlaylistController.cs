using API.Extensions;
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

    [HttpGet("paged", Name = "GetPlaylistsPaged")]
    public async Task<ActionResult<PagedResultDto<PlaylistDto>>> GetPaged(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] string? creatorId,
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12
    )
    {
        var filter = new PlaylistFilterDto
        {
            Name = name,
            Description = description,
            CreatorId = creatorId,
            SortBy = sortBy,
            SortDescending = sortDescending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var parsedFromDate))
        {
            filter.FromDate = parsedFromDate;
        }
        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var parsedToDate))
        {
            filter.ToDate = parsedToDate;
        }

        var result = await _playlistService.GetByFilterPagedAsync(filter);
        return Ok(result);
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

    [HttpGet("{id:int}/videos", Name = "GetPlaylistWithVideos")]
    public async Task<ActionResult<object>> GetWithVideos(int id)
    {
        var result = await _playlistService.GetByIdWithVideosAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        var (playlist, videos) = result.Value;

        return Ok(new
        {
            Playlist = playlist,
            Videos = videos
        });
    }

    [HttpPost(Name = "CreatePlaylist")]
    public async Task<ActionResult<PlaylistDto>> Create([FromBody] PlaylistCreateDto dto)
    {
        var result = await _playlistService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetPlaylistById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPost("{playlistId:int}/videos/{videoId:int}", Name = "AddVideoToPlaylist")]
    public async Task<IActionResult> AddVideo(int playlistId, int videoId)
    {
        var result = await _playlistService.AddVideoAsync(playlistId, videoId);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return NoContent();
    }

    [HttpDelete("{playlistId:int}/videos/{videoId:int}", Name = "RemoveVideoFromPlaylist")]
    public async Task<IActionResult> RemoveVideo(int playlistId, int videoId)
    {
        var result = await _playlistService.RemoveVideoAsync(playlistId, videoId);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdatePlaylist")]
    public async Task<ActionResult<PlaylistDto>> Update(int id, [FromBody] PlaylistUpdateDto dto)
    {
        dto.Id = id;
        var result = await _playlistService.UpdateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}", Name = "DeletePlaylist")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _playlistService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
