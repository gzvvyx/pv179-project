using Business.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllAsync();
        Task<CommentDto?> GetByIdAsync(int id);
        Task<(IdentityResult Result, CommentDto? Comment)> CreateAsync(CommentCreateDto dto);
        Task<(IdentityResult Result, CommentDto? Comment)> UpdateAsync(int id, CommentUpdateDto dto);
        Task<IdentityResult> DeleteAsync(int id);
    }
}
