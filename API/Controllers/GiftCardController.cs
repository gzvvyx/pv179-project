using API.Extensions;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftCardController : ControllerBase
{
    private readonly ILogger<GiftCardController> _logger;
    private readonly IGiftCardService _giftCardService;
    private readonly IGiftCardCodeService _giftCardCodeService;

    public GiftCardController(
        ILogger<GiftCardController> logger, 
        IGiftCardService giftCardService,
        IGiftCardCodeService giftCardCodeService)
    {
        _logger = logger;
        _giftCardService = giftCardService;
        _giftCardCodeService = giftCardCodeService;
    }

    [HttpGet(Name = "GetGiftCards")]
    public async Task<IEnumerable<GiftCardDto>> Get()
    {
        return await _giftCardService.GetAllAsync();
    }

    [HttpGet("{id:int}", Name = "GetGiftCardById")]
    public async Task<ActionResult<GiftCardDto>> GetById(int id)
    {
        var giftCard = await _giftCardService.GetByIdAsync(id);

        if (giftCard is null)
        {
            return NotFound();
        }

        return Ok(giftCard);
    }

    [HttpPost(Name = "CreateGiftCard")]
    public async Task<ActionResult<GiftCardDto>> Create([FromBody] GiftCardCreateDto dto)
    {
        var result = await _giftCardService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetGiftCardById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateGiftCard")]
    public async Task<ActionResult<GiftCardDto>> Update(int id, [FromBody] GiftCardUpdateDto dto)
    {
        dto.Id = id;
        var result = await _giftCardService.UpdateAsync(dto);

        return result.ToActionResult();
    }

    [HttpDelete("{id:int}", Name = "DeleteGiftCard")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _giftCardService.DeleteAsync(id);

        return result.ToActionResult();
    }

    [HttpPost("{id:int}/codes")]
    public async Task<ActionResult<GiftCardCodeDto>> CreateCode(int id, [FromBody] GiftCardCodeCreateDto dto)
    {
        dto.GiftCardId = id;

        var result = await _giftCardCodeService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetGiftCardCodeByCode", new { code = result.Value.Code }, result.Value);
    }

    [HttpDelete("{id:int}/codes/{code}")]
    public async Task<IActionResult> DeleteCode(int id, string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Code is required.");
        }

        var result = await _giftCardCodeService.DeleteAsync(code);

        return result.ToActionResult();
    }
}