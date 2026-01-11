using API.Extensions;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly ICategoryService _categoryService;

    public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<IEnumerable<CategoryDto>> Get()
    {
        return await _categoryService.GetAllAsync();
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("by-name/{name}", Name = "GetCategoryByName")]
    public async Task<ActionResult<CategoryDto>> GetByName(string name)
    {
        var category = await _categoryService.GetByNameAsync(name);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost(Name = "CreateCategory")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryCreateDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetCategoryById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateCategory")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        dto.Id = id;
        var result = await _categoryService.UpdateAsync(dto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }
}

