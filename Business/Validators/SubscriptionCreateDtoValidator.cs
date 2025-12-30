using Business.DTOs;
using DAL.Models.Enums;
using FluentValidation;

namespace Business.Validators;

public class SubscriptionCreateDtoValidator : AbstractValidator<SubscriptionCreateDto>
{
    public SubscriptionCreateDtoValidator()
    {
        RuleFor(x => x.OrdererId)
            .NotEmpty()
            .WithMessage("OrdererId is required.");

        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.Active)
            .NotNull()
            .WithMessage("Active is required.");

        RuleFor(x => x.Timeframe)
            .NotNull()
            .WithMessage("Timeframe is required.")
            .Must(timeframe => Enum.IsDefined(typeof(SubscriptionTimeframe), timeframe))
            .WithMessage("Timeframe must be a valid SubscriptionTimeframe value.");
    }
}

