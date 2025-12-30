using API.Extensions;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{

    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
    }

    [HttpGet(Name = "GetSubscriptions")]
    public async Task<IEnumerable<SubscriptionDto>> Get()
    {
        return await _subscriptionService.GetAllAsync();
    }

    [HttpGet("with-users", Name = "GetSubscriptionsWithUsers")]
    public async Task<IEnumerable<SubscriptionWithUsersDto>> GetAllWithUsers()
    {
        return await _subscriptionService.GetAllWithUsersAsync();
    }

    [HttpGet("{id:int}", Name = "GetSubscriptionById")]
    public async Task<ActionResult<SubscriptionDto>> GetById(int id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);

        if (subscription is null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }

    [HttpPost(Name = "CreateSubscription")]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] SubscriptionCreateDto subscriptionDto)
    {
        var result = await _subscriptionService.CreateAsync(subscriptionDto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetSubscriptionById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateSubscription")]
    public async Task<ActionResult<SubscriptionDto>> Update(int id, [FromBody] SubscriptionUpdateDto subscriptionDto)
    {
        subscriptionDto.Id = id;
        var result = await _subscriptionService.UpdateAsync(subscriptionDto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}", Name = "DeleteSubscription")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriptionService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
