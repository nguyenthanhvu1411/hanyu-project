using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Common.Middleware;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler>
        _logger;

    private readonly IHostEnvironment
        _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (
            statusCode,
            title,
            detail,
            code) =
            MapException(exception);

        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }

        var problem =
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail =
                    _environment.IsDevelopment()
                        ? detail
                        : GetProductionDetail(
                            statusCode),
                Instance =
                    httpContext.Request.Path
            };

        problem.Extensions["code"] =
            code;

        problem.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode =
            statusCode;

        await httpContext.Response
            .WriteAsJsonAsync(
                problem,
                cancellationToken);

        return true;
    }

    private static (
        int StatusCode,
        string Title,
        string Detail,
        string Code)
        MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    exception.Message,
                    "Common.Validation"
                ),

            UnauthorizedAccessException =>
                (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    exception.Message,
                    "Common.Unauthorized"
                ),

            KeyNotFoundException =>
                (
                    StatusCodes.Status404NotFound,
                    "Resource not found",
                    exception.Message,
                    "Common.NotFound"
                ),

            InvalidOperationException =>
                (
                    StatusCodes.Status409Conflict,
                    "Operation conflict",
                    exception.Message,
                    "Common.Conflict"
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "Internal server error",
                    exception.Message,
                    "Common.InternalServerError"
                )
        };
    }

    private static string GetProductionDetail(
        int statusCode)
    {
        return statusCode >= 500
            ? "An unexpected error occurred."
            : "The request could not be completed.";
    }
}
