using HanYu.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Common.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(
                result.Value);
        }

        return controller.ToProblem(
            result.Error);
    }

    public static IActionResult ToActionResult(
        this ControllerBase controller,
        Result result)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return controller.ToProblem(
            result.Error);
    }

    public static IActionResult ToProblem(
        this ControllerBase controller,
        Error error)
    {
        var statusCode =
            error.Type switch
            {
                ErrorType.Validation =>
                    StatusCodes.Status400BadRequest,

                ErrorType.Unauthorized =>
                    StatusCodes.Status401Unauthorized,

                ErrorType.Forbidden =>
                    StatusCodes.Status403Forbidden,

                ErrorType.NotFound =>
                    StatusCodes.Status404NotFound,

                ErrorType.Conflict =>
                    StatusCodes.Status409Conflict,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(error.Type),
            Detail = error.Message
        };

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static string GetTitle(
        ErrorType type)
    {
        return type switch
        {
            ErrorType.Validation =>
                "Validation failed",

            ErrorType.Unauthorized =>
                "Unauthorized",

            ErrorType.Forbidden =>
                "Forbidden",

            ErrorType.NotFound =>
                "Resource not found",

            ErrorType.Conflict =>
                "Conflict",

            _ =>
                "Request failed"
        };
    }
}
