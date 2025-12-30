using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ErrorOrExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ErrorOr<T> result)
    {
        if (!result.IsError)
        {
            return new OkObjectResult(result.Value);
        }

        var firstError = result.FirstError;

        return firstError.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.NotFound => new NotFoundObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Conflict => new ConflictObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Unauthorized => new UnauthorizedObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Forbidden => new ObjectResult(
                result.Errors.Select(e => e.Description).ToList())
            {
                StatusCode = 403
            },
            
            _ => new BadRequestObjectResult(
                result.Errors.Select(e => e.Description).ToList())
        };
    }

    public static IActionResult ToActionResult(this ErrorOr<Success> result)
    {
        if (!result.IsError)
        {
            return new NoContentResult();
        }

        var firstError = result.FirstError;

        return firstError.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.NotFound => new NotFoundObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Conflict => new ConflictObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Unauthorized => new UnauthorizedObjectResult(
                result.Errors.Select(e => e.Description).ToList()),
            
            ErrorType.Forbidden => new ObjectResult(
                result.Errors.Select(e => e.Description).ToList())
            {
                StatusCode = 403
            },
            
            _ => new BadRequestObjectResult(
                result.Errors.Select(e => e.Description).ToList())
        };
    }
}

