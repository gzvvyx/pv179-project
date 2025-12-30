using Business.DTOs;
using DAL.Models.Enums;
using FluentValidation;

namespace Business.Validators;

public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Amount is required.")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("Status is required.")
            .Must(status => status.HasValue && Enum.IsDefined(typeof(OrderStatus), status.Value))
            .WithMessage("Status must be a valid OrderStatus value (Pending, Failed, or Completed).");
    }
}

