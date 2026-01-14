using Business.DTOs;
using ErrorOr;
using Infra.DTOs;

namespace Business.Services
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllAsync();
        Task<CommentDto?> GetByIdAsync(int id);
        Task<List<CommentDto>> GetByVideoIdAsync(int videoId);
        Task<ErrorOr<CommentDto>> CreateAsync(CommentCreateDto dto);
        Task<ErrorOr<CommentDto>> UpdateAsync(CommentUpdateDto dto);
        Task<ErrorOr<Success>> DeleteAsync(int id);
    }
}
