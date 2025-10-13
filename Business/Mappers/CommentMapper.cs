using Business.DTOs;
using DAL.Models;
using Riok.Mapperly.Abstractions;

namespace Business.Mappers
{
    [Mapper]
    public partial class CommentMapper
    {
        public partial List<CommentDto> Map(List<Comment> comments);
        public partial CommentDto Map(Comment comment);
    }
}
