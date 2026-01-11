using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.PricePerMonth)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PricePerMonth.HasValue)
            .WithMessage("PricePerMonth must be greater than or equal to 0.");
    }
}

