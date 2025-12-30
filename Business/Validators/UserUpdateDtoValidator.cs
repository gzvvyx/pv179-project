using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .When(x => !string.IsNullOrWhiteSpace(x.NewPassword))
            .WithMessage("The password and confirmation password do not match.");

        RuleFor(x => x.NewPassword)
            .MinimumLength(6)
            .When(x => !string.IsNullOrWhiteSpace(x.NewPassword))
            .WithMessage("Password must be at least 6 characters long.");
    }
}

