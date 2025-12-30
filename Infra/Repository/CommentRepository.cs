using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _dbContext;

        public CommentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Comment>> GetAllAsync()
        {
            return _dbContext.Comments
                .AsNoTracking()
                .Include(comment => comment.Author)
                .ToListAsync();
        }

        public Task<Comment?> GetByIdAsync(int id)
        {
            return _dbContext.Comments
                .Include(comment => comment.Author)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task CreateAsync(Comment comment)
        {
            if (comment.Author is not null)
            {
                _dbContext.Attach(comment.Author);
            }

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {

            if (comment.Author is not null)
            {
                _dbContext.Attach(comment.Author);
            }

            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
