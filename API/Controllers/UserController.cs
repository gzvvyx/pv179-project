using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;

    }

    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<UserDto>> Get()
    {
        return await _userService.GetAllAsync();
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, user) = await _userService.CreateAsync(dto);

        if (!result.Succeeded || user is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, user) = await _userService.UpdateAsync(id, dto);

        if (result.Succeeded && user is not null)
        {
            return Ok(user);
        }

        if (result.Errors.Any(error => error.Code == "UserNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "UserNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}
