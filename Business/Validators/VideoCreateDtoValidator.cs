using Business.DTOs;
using FluentValidation;

namespace Business.Validators;

public class VideoCreateDtoValidator : AbstractValidator<VideoCreateDto>
{
    public VideoCreateDtoValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(255)
            .WithMessage("Title cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Url is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Url must be a valid URL.");

        RuleFor(x => x.ThumbnailUrl)
            .NotEmpty()
            .WithMessage("ThumbnailUrl is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("ThumbnailUrl must be a valid URL.");
    }
}

