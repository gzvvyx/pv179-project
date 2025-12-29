using Business.DTOs;
using Business.Mappers;
using Infra.Repository;

namespace Business.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _mapper = new();

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
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
}
