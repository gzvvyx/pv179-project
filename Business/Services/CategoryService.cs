using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.Repository;

namespace Business.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CategoryCreateDto> _createValidator;
    private readonly IValidator<CategoryUpdateDto> _updateValidator;
    private readonly CategoryMapper _mapper = new();

    public CategoryService(
        ICategoryRepository categoryRepository,
        IValidator<CategoryCreateDto> createValidator,
        IValidator<CategoryUpdateDto> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is null ? null : _mapper.Map(category);
    }

    public async Task<CategoryDto?> GetByNameAsync(string name)
    {
        var category = await _categoryRepository.GetByNameAsync(name);
        return category is null ? null : _mapper.Map(category);
    }

    public async Task<ErrorOr<CategoryDto>> CreateAsync(CategoryCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        // Check if category with same name already exists
        var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);
        if (existingCategory != null)
        {
            return Error.Conflict(description: "A category with this name already exists.");
        }

        var category = new Category
        {
            Id = default,
            Name = dto.Name,
            CreatedAt = default,
            UpdatedAt = default
        };

        await _categoryRepository.CreateAsync(category);

        return _mapper.Map(category);
    }

    public async Task<ErrorOr<CategoryDto>> UpdateAsync(CategoryUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category is null)
        {
            return Error.NotFound();
        }

        // Check if another category with the same name exists (if name is being changed)
        if (dto.Name != null && dto.Name != category.Name)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);
            if (existingCategory != null && existingCategory.Id != dto.Id)
            {
                return Error.Conflict(description: "A category with this name already exists.");
            }
        }

        if (dto.Name != null)
        {
            category.Name = dto.Name;
        }

        await _categoryRepository.UpdateAsync(category);

        return _mapper.Map(category);
    }
}
