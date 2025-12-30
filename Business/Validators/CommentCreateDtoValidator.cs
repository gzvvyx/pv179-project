using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
{
    public CommentCreateDtoValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("AuthorId is required.");

        RuleFor(x => x.VideoId)
            .GreaterThan(0)
            .WithMessage("VideoId must be greater than 0.");

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Content is required.")
            .NotEmpty()
            .WithMessage("Content cannot be empty.")
            .MaximumLength(500)
            .WithMessage("Content cannot exceed 500 characters.");
    }
}

