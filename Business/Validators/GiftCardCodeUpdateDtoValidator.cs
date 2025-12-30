using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class GiftCardCodeUpdateDtoValidator : AbstractValidator<GiftCardCodeUpdateDto>
{
    public GiftCardCodeUpdateDtoValidator()
    {
    }
}

