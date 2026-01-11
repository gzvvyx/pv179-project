using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class GiftCardCreateDtoValidator : AbstractValidator<GiftCardCreateDto>
{
    public GiftCardCreateDtoValidator()
    {
        RuleFor(x => x.PriceReduction)
            .GreaterThan(0)
            .WithMessage("PriceReduction must be greater than 0.");

        RuleFor(x => x.ValidFrom)
            .NotEmpty()
            .WithMessage("ValidFrom is required.");

        RuleFor(x => x.ValidTo)
            .NotEmpty()
            .WithMessage("ValidTo is required.")
            .GreaterThan(x => x.ValidFrom)
            .WithMessage("ValidTo must be greater than ValidFrom.");
    }
}

