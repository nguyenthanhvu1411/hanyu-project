using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Account;

namespace HanYu.Application.Interfaces.Authentication;

public interface IAccountService
{
    Task<Result<AccountResponse>> ChangeEmailAsync(
        Guid userId,
        ChangeEmailRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<Result<AccountResponse>> ChangeUsernameAsync(
        Guid userId,
        ChangeUsernameRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid userId,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreAsync(
        Guid publicId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);
}
