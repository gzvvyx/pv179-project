using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class VideoUpdateDtoValidator : AbstractValidator<VideoUpdateDto>
{
    public VideoUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Title)
            .MaximumLength(255)
            .WithMessage("Title cannot exceed 255 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Url)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Url must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.Url));

        RuleFor(x => x.ThumbnailUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("ThumbnailUrl must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl));
    }
}

