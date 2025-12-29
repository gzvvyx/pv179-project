using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Category?> GetByNameAsync(string name)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}
