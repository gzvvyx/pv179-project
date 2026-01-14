using Business.DTOs;
using Business.Extensions;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.DTOs;
using Infra.Repository;

namespace Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CommentCreateDto> _createValidator;
        private readonly IValidator<CommentUpdateDto> _updateValidator;
        private readonly AppDbContext _dbContext;
        private readonly CommentMapper _mapper = new();

        public CommentService(
            ICommentRepository commentRepository, 
            IVideoRepository videoRepository, 
            IUserRepository userRepository,
            AppDbContext dbContext,
            IValidator<CommentCreateDto> createValidator,
            IValidator<CommentUpdateDto> updateValidator)
        {
            _commentRepository = commentRepository;
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<CommentDto>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return _mapper.Map(comments);
        }

        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment == null ? null : _mapper.Map(comment);
        }

        public async Task<List<CommentDto>> GetByVideoIdAsync(int videoId)
        {
            var allComments = await _commentRepository.GetByVideoIdAsync(videoId);
            
            var commentDtos = _mapper.Map(allComments);
            
            var commentDict = commentDtos.ToDictionary(c => c.Id);
            var rootComments = new List<CommentDto>();
            
            foreach (var comment in commentDtos)
            {
                if (comment.ParentCommentId == null)
                {
                    rootComments.Add(comment);
                }
                else if (commentDict.TryGetValue(comment.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(comment);
                }
            }
            
            return rootComments;
        }

        public async Task<ErrorOr<CommentDto>> CreateAsync(CommentCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return validationResult.ToErrors();
            }

            var author = await _userRepository.GetByIdAsync(dto.AuthorId);
            if (author is null)
            {
                return Error.NotFound();
            }

            var video = await _videoRepository.GetByIdAsync(dto.VideoId);
            if (video is null)
            {
                return Error.NotFound();
            }

            var comment = new Comment
            {
                Id = default,
                VideoId = dto.VideoId,
                Video = video,
                AuthorId = dto.AuthorId,
                Author = author,
                Content = dto.Content!,
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = default,
                UpdatedAt = default
            };

            await _commentRepository.CreateAsync(comment);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map(comment);
        }

        public async Task<ErrorOr<CommentDto>> UpdateAsync(CommentUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return validationResult.ToErrors();
            }

            var comment = await _commentRepository.GetByIdAsync(dto.Id);
            if (comment is null)
            {
                return Error.NotFound();
            }

            if (dto.Content != null)
            {
                comment.Content = dto.Content;
            }

            await _commentRepository.UpdateAsync(comment);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map(comment);
        }

        public async Task<ErrorOr<Success>> DeleteAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                return Error.NotFound();
            }

            await _commentRepository.DeleteAsync(comment);
            await _dbContext.SaveChangesAsync();

            return Result.Success;
        }
    }
}
