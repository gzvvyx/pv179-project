using ErrorOr;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MVC.Extensions;

public static class ErrorOrExtensions
{
    public static void AddErrorsToModelState<T>(this ErrorOr<T> result, ModelStateDictionary modelState)
    {
        if (!result.IsError)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.Code, error.Description);
        }
    }
}

