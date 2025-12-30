using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class CommentUpdateDtoValidator : AbstractValidator<CommentUpdateDto>
{
    public CommentUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Content is required.")
            .NotEmpty()
            .WithMessage("Content cannot be empty.")
            .MaximumLength(500)
            .WithMessage("Content cannot exceed 500 characters.");
    }
}

