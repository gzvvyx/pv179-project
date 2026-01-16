using API.Extensions;
using API.DTOs;
using API.Mappers;
using Business.DTOs;
using Business.Services;
using Infra.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly UserRequestMapper _mapper = new();

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<UserDto>> Get([FromQuery] GetUsersRequestDto request)
    {
        var filter = _mapper.ToFilterDto(request);
        return await _userService.GetByFilterAsync(filter);
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

    [HttpGet("paged", Name = "GetUsersPaged")]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetPaged([FromQuery] GetUsersPagedRequestDto request)
    {
        var filter = _mapper.ToFilterDto(request);
        var result = await _userService.GetByFilterPagedAsync(filter);
        return Ok(result);
    }

    [HttpPost(Name = "CreateUser")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto dto)
    {
        var result = await _userService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetUserById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}", Name = "UpdateUser")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UserUpdateDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);

        return result.ToActionResult();
    }

    [HttpDelete("{id}", Name = "DeleteUser")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
