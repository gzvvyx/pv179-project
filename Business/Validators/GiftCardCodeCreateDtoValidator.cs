using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class GiftCardCodeCreateDtoValidator : AbstractValidator<GiftCardCodeCreateDto>
{
    public GiftCardCodeCreateDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(20)
            .WithMessage("Code must not exceed 20 characters.");

        RuleFor(x => x.GiftCardId)
            .GreaterThan(0)
            .WithMessage("GiftCardId must be greater than 0.");
    }
}

