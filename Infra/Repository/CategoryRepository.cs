using DAL.Data;
using DAL.Models;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class CategoryRepository : ICategoryRepository
{
    private const string CategoriesGetAllCacheKey = "Categories_GetAll";
    
    private readonly AppDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public CategoryRepository(AppDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _cacheService.GetOrSetAsync(CategoriesGetAllCacheKey, async () =>
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        });
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        return _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Category?> GetByNameAsync(string name)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task CreateAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        _cacheService.Remove(CategoriesGetAllCacheKey);
    }

    public async Task UpdateAsync(Category category)
    {
        _dbContext.Categories.Update(category);
        _cacheService.Remove(CategoriesGetAllCacheKey);
    }

    public async Task DeleteAsync(Category category)
    {
        _dbContext.Categories.Remove(category);
        _cacheService.Remove(CategoriesGetAllCacheKey);
    }
}
