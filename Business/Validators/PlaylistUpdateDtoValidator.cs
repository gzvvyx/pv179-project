using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class PlaylistUpdateDtoValidator : AbstractValidator<PlaylistUpdateDto>
{
    public PlaylistUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name is required.")
            .NotEmpty()
            .WithMessage("Name cannot be empty.");
    }
}

