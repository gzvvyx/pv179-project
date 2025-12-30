using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Business.Extensions;

public static class ValidationResultExtensions
{
    public static Error? ToError(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return null;
        }

        var firstError = validationResult.Errors.First();
        return Error.Validation(firstError.PropertyName, firstError.ErrorMessage);
    }

    public static List<Error> ToErrors(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return new List<Error>();
        }

        return validationResult.Errors
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .ToList();
    }

    public static ErrorOr<T> ToErrorOr<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return default(T)!;
        }

        return validationResult.ToErrors();
    }

    public static List<Error> ToErrors(this IdentityResult identityResult)
    {
        if (identityResult.Succeeded)
        {
            return new List<Error>();
        }

        return identityResult.Errors
            .Select(error => Error.Validation(error.Code, error.Description))
            .ToList();
    }
}

