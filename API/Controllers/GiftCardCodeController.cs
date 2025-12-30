using API.Extensions;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftCardCodeController : ControllerBase
{
    private readonly ILogger<GiftCardCodeController> _logger;
    private readonly IGiftCardCodeService _giftCardCodeService;

    public GiftCardCodeController(ILogger<GiftCardCodeController> logger, IGiftCardCodeService giftCardCodeService)
    {
        _logger = logger;
        _giftCardCodeService = giftCardCodeService;
    }

    [HttpGet(Name = "GetGiftCardCodes")]
    public async Task<IEnumerable<GiftCardCodeDto>> Get()
    {
        return await _giftCardCodeService.GetAllAsync();
    }

    [HttpGet("{code}", Name = "GetGiftCardCodeByCode")]
    public async Task<ActionResult<GiftCardCodeDto>> GetByCode(string code)
    {
        var giftCardCode = await _giftCardCodeService.GetByCodeAsync(code);
        if (giftCardCode is null)
        {
            return NotFound();
        }
        return Ok(giftCardCode);
    }

    [HttpPost(Name = "CreateGiftCardCode")]
    public async Task<ActionResult<GiftCardCodeDto>> Create([FromBody] GiftCardCodeCreateDto dto)
    {
        var result = await _giftCardCodeService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetGiftCardCodeByCode", new { code = result.Value.Code }, result.Value);
    }

    [HttpPut("{code}", Name = "UpdateGiftCardCode")]
    public async Task<ActionResult<GiftCardCodeDto>> Update(string code, [FromBody] GiftCardCodeUpdateDto dto)
    {
        var result = await _giftCardCodeService.UpdateAsync(code, dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{code}", Name = "DeleteGiftCardCode")]
    public async Task<IActionResult> Delete(string code)
    {
        var result = await _giftCardCodeService.DeleteAsync(code);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return NoContent();
    }
}