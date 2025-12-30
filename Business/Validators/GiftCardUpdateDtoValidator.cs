using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class GiftCardUpdateDtoValidator : AbstractValidator<GiftCardUpdateDto>
{
    public GiftCardUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.PriceReduction)
            .GreaterThan(0)
            .When(x => x.PriceReduction.HasValue)
            .WithMessage("PriceReduction must be greater than 0 when provided.");
    }
}

