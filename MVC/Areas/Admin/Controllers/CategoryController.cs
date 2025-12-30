using Business.DTOs;
using Business.Services;
using DAL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Admin.Models;
using MVC.Extensions;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(
        ICategoryService categoryService,
        ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [Route("", Name = "AdminCategories")]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        var viewModels = categories.Select(c => new CategoryIndexViewModel
        {
            Id = c.Id,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Create")]
    public IActionResult Create()
    {
        return View(new CreateCategoryViewModel());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var categoryCreateDto = new CategoryCreateDto
        {
            Name = model.Name
        };

        var result = await _categoryService.CreateAsync(categoryCreateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            return View(model);
        }

        TempData["SuccessMessage"] = "Category created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var model = new EditCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            model.CreatedAt = category.CreatedAt;
            model.UpdatedAt = category.UpdatedAt;
            return View(model);
        }

        var categoryUpdateDto = new CategoryUpdateDto
        {
            Id = id,
            Name = model.Name
        };

        var result = await _categoryService.UpdateAsync(categoryUpdateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            var category = await _categoryService.GetByIdAsync(id);
            if (category != null)
            {
                model.CreatedAt = category.CreatedAt;
                model.UpdatedAt = category.UpdatedAt;
            }
            return View(model);
        }

        TempData["SuccessMessage"] = "Category updated successfully!";
        return RedirectToAction(nameof(Index));
    }
}

