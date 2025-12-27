using Business.DTOs;
using Business.Mappers;
using DAL.Models;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly CommentMapper _mapper = new();

        public CommentService(ICommentRepository commentRepository, IVideoRepository videoRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _videoRepository = videoRepository;
            _userRepository = userRepository;
        }

        public async Task<List<CommentDto>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return _mapper.Map(comments);
        }

        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment is null ? null : _mapper.Map(comment);
        }

        public async Task<(IdentityResult Result, CommentDto? Comment)> CreateAsync(CommentCreateDto dto)
        {
            var creator = await _userRepository.GetByIdAsync(dto.AuthorId);
            if (creator is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "CreatorNotFound",
                    Description = $"Creator with id '{dto.AuthorId}' was not found."
                }), null);
            }

            var video = await _videoRepository.GetByIdAsync(dto.VideoId);
            if (video is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "VideoNotFound",
                    Description = $"Video with id '{dto.VideoId}' was not found."
                }), null);
            }

            var timestamp = DateTime.UtcNow;

            var comment = new Comment
            {
                Id = default,
                VideoId = dto.VideoId,
                Video = video,
                AuthorId = dto.AuthorId,
                Author = creator,
                Content = dto.Content,
                CreatedAt = timestamp,
                UpdatedAt = timestamp
            };

            await _commentRepository.CreateAsync(comment);

            return (IdentityResult.Success, _mapper.Map(comment));
        }

        public async Task<(IdentityResult Result, CommentDto? Comment)> UpdateAsync(int id, CommentUpdateDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "CommentNotFound",
                    Description = $"Comment with id '{id}' was not found."
                }), null);
            }
            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateAsync(comment);
            return (IdentityResult.Success, _mapper.Map(comment));
        }

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CommentNotFound",
                    Description = $"Comment with id '{id}' was not found."
                });
            }
            await _commentRepository.DeleteAsync(comment);
            return IdentityResult.Success;
        }
    }
}
