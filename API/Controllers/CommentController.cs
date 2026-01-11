using API.Extensions;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;

    public CommentController(ILogger<CommentController> logger, ICommentService commentService)
    {
        _logger = logger;
        _commentService = commentService;
    }

    [HttpGet(Name = "GetComments")]
    public async Task<IEnumerable<CommentDto>> Get()
    {
        return await _commentService.GetAllAsync();
    }

    [HttpGet("{id:int}", Name = "GetCommentById")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        var comment = await _commentService.GetByIdAsync(id);

        if (comment is null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpGet("by-video/{videoId:int}", Name = "GetCommentsByVideoId")]
    public async Task<IEnumerable<CommentDto>> GetByVideoId(int videoId)
    {
        return await _commentService.GetByVideoIdAsync(videoId);
    }

    [HttpPost(Name = "CreateComment")]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CommentCreateDto dto)
    {
        var result = await _commentService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetCommentById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateComment")]
    public async Task<ActionResult<CommentDto>> Update(int id, [FromBody] CommentUpdateDto dto)
    {
        dto.Id = id;
        var result = await _commentService.UpdateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}", Name = "DeleteComment")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commentService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
