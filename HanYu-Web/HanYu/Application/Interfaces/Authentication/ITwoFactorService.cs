using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Features.Identity.Login;
using HanYu.Application.Features.Identity.TwoFactor.Common;

namespace HanYu.Application.Interfaces.Authentication;

public interface ITwoFactorService
{
    Task<Result<TwoFactorSetupResponse>> SetupAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<TwoFactorRecoveryCodesResponse>> EnableAsync(
        Guid userId,
        string code,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<Result> DisableAsync(
        Guid userId,
        string password,
        string code,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<Result<TwoFactorRecoveryCodesResponse>>
        GenerateRecoveryCodesAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> CompleteLoginAsync(
        string challengeToken,
        string code,
        LoginClientInfo client,
        CancellationToken cancellationToken = default);

    Task<Result<TwoFactorSetupResponse>>
        RegenerateAuthenticatorKeyAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken = default);
}
