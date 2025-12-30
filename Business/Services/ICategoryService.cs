using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto?> GetByNameAsync(string name);
    Task<ErrorOr<CategoryDto>> CreateAsync(CategoryCreateDto dto);
    Task<ErrorOr<CategoryDto>> UpdateAsync(CategoryUpdateDto dto);
}
