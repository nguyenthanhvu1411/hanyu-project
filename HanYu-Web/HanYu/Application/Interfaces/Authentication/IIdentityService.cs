using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;

namespace HanYu.Application.Interfaces.Authentication;

public interface IIdentityService
{
    Task<Result<AuthResponse>> RegisterAsync(
        RegisterIdentityRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginAsync(
        LoginIdentityRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshIdentityTokenRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<CurrentUserResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    // ============================================================
    // Email verification
    // ============================================================

    Task<Result<string>> GenerateEmailVerificationTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    // ============================================================
    // Password recovery
    // ============================================================

    Task<Result<string?>> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityEmailUser?>> FindUserForEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}

public sealed record RegisterIdentityRequest(
    string UserName,
    string Email,
    string Password,
    string DisplayName);

public sealed record LoginIdentityRequest(
    string Email,
    string Password,
    string? IpAddress,
    string? UserAgent,
    string? DeviceName,
    string? DeviceType,
    string? Browser,
    string? OperatingSystem);

public sealed record RefreshIdentityTokenRequest(
    string RefreshToken,
    string? IpAddress,
    string? UserAgent);

public sealed record IdentityEmailUser(
    Guid UserId,
    string Email,
    string DisplayName);