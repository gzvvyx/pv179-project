using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.OrdererId)
            .NotEmpty()
            .WithMessage("OrdererId is required.");

        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Amount is required.")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");
    }
}

