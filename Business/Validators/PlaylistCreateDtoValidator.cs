using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class PlaylistCreateDtoValidator : AbstractValidator<PlaylistCreateDto>
{
    public PlaylistCreateDtoValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name is required.")
            .NotEmpty()
            .WithMessage("Name cannot be empty.");
    }
}

