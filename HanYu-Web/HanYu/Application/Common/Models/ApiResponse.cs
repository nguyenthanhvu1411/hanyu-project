namespace HanYu.Application.Common.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public T? Data { get; init; }

    public ApiError? Error { get; init; }

    public static ApiResponse<T> Ok(
        T data,
        string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(
        string code,
        string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError
            {
                Code = code,
                Message = message
            }
        };
    }
}

public sealed class ApiResponse
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public ApiError? Error { get; init; }

    public static ApiResponse Ok(
        string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse Fail(
        string code,
        string message)
    {
        return new ApiResponse
        {
            Success = false,
            Error = new ApiError
            {
                Code = code,
                Message = message
            }
        };
    }
}

public sealed class ApiError
{
    public required string Code { get; init; }

    public required string Message { get; init; }
}
