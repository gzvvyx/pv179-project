using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

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
}