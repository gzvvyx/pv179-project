using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null)
            .WithMessage("Name cannot be empty.")
            .MaximumLength(100)
            .When(x => x.Name != null)
            .WithMessage("Name must not exceed 100 characters.");
    }
}

