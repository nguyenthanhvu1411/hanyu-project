namespace HanYu.Application.Common.Models;

public enum ErrorType
{
    None = 0,

    Validation = 10,

    NotFound = 20,

    Conflict = 30,

    Unauthorized = 40,

    Forbidden = 50,

    Failure = 60
}
