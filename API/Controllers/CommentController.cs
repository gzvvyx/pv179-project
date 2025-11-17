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

    [HttpPost(Name = "CreateComment")]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CommentCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, comment) = await _commentService.CreateAsync(dto);

        if (!result.Succeeded || comment is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetCommentById", new { id = comment.Id }, comment);
    }

    [HttpPut("{id:int}", Name = "UpdateComment")]
    public async Task<ActionResult<CommentDto>> Update(int id, [FromBody] CommentUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, comment) = await _commentService.UpdateAsync(id, dto);

        if (result.Succeeded || comment is not null)
        {
            return Ok(comment);
        }

        if (result.Errors.Any(error => error.Code == "CommentNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}", Name = "DeleteComment")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commentService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "CommentNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}
