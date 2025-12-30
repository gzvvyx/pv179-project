using Business.DTOs;
using DAL.Models.Enums;
using FluentValidation;

namespace Business.Validators;

public class SubscriptionUpdateDtoValidator : AbstractValidator<SubscriptionUpdateDto>
{
    public SubscriptionUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");
    }
}

