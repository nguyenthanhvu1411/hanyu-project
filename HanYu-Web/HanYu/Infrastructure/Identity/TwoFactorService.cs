using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Features.Identity.Login;
using HanYu.Application.Features.Identity.TwoFactor.Common;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class TwoFactorService : ITwoFactorService
{
    private static readonly string AuthenticatorProvider =
        TokenOptions.DefaultAuthenticatorProvider;

    private readonly UserManager<User> _userManager;
    private readonly HanYuDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISecurityEventService _securityEventService;
    private readonly IAuthenticationTokenIssuer _tokenIssuer;

    public TwoFactorService(
        UserManager<User> userManager,
        HanYuDbContext dbContext,
        IJwtTokenService jwtTokenService,
        ISecurityEventService securityEventService,
        IAuthenticationTokenIssuer tokenIssuer)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _securityEventService = securityEventService;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<Result<TwoFactorSetupResponse>> SetupAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.TwoFactorEnabled)
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.Conflict(
                    "Identity.TwoFactorAlreadyEnabled",
                    "Xác thực hai bước đã được bật."));
        }

        var key =
            await _userManager
                .GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrWhiteSpace(key))
        {
            var resetResult =
                await _userManager
                    .ResetAuthenticatorKeyAsync(
                        user);

            if (!resetResult.Succeeded)
            {
                var message =
                    string.Join(
                        "; ",
                        resetResult.Errors.Select(
                            x => x.Description));

                return Result.Failure<TwoFactorSetupResponse>(
                    Error.Validation(
                        "Identity.Validation",
                        message));
            }

            key =
                await _userManager
                    .GetAuthenticatorKeyAsync(
                        user);
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.Validation(
                    "Identity.AuthenticatorKeyGenerationFailed",
                    "Không thể tạo khóa xác thực."));
        }

        var email =
            user.Email
            ?? user.UserName
            ?? user.Id.ToString();

        var uri =
            GenerateOtpUri(
                "HanYu",
                email,
                key);

        return Result.Success(
            new TwoFactorSetupResponse(
                FormatKey(key),
                uri));
    }

    public async Task<Result<TwoFactorRecoveryCodesResponse>> EnableAsync(
        Guid userId,
        string code,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.TwoFactorEnabled)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Conflict(
                    "Identity.TwoFactorAlreadyEnabled",
                    "Xác thực hai bước đã được bật."));
        }

        var normalizedCode =
            NormalizeAuthenticatorCode(code);

        var valid =
            await _userManager
                .VerifyTwoFactorTokenAsync(
                    user,
                    AuthenticatorProvider,
                    normalizedCode);

        if (!valid)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Validation(
                    "Identity.InvalidTwoFactorCode",
                    "Mã xác thực không hợp lệ."));
        }

        var enableResult =
            await _userManager
                .SetTwoFactorEnabledAsync(
                    user,
                    true);

        if (!enableResult.Succeeded)
        {
            var message =
                string.Join(
                    "; ",
                    enableResult.Errors.Select(
                        x => x.Description));

            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Validation(
                    "Identity.Validation",
                    message));
        }

        user.EnableTwoFactor();

        var recoveryCodes =
            await _userManager
                .GenerateNewTwoFactorRecoveryCodesAsync(
                    user,
                    10);

        var codes =
            recoveryCodes?
                .ToArray()
            ?? Array.Empty<string>();

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.TwoFactorEnabled,
            ipAddress,
            userAgent,
            cancellationToken:
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new TwoFactorRecoveryCodesResponse(
                codes));
    }

    public async Task<Result> DisableAsync(
        Guid userId,
        string password,
        string code,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (!user.TwoFactorEnabled)
        {
            return Result.Success();
        }

        var passwordValid =
            await _userManager
                .CheckPasswordAsync(
                    user,
                    password);

        if (!passwordValid)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        var validCode =
            await _userManager
                .VerifyTwoFactorTokenAsync(
                    user,
                    AuthenticatorProvider,
                    NormalizeAuthenticatorCode(
                        code));

        if (!validCode)
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidTwoFactorCode",
                    "Mã xác thực không hợp lệ."));
        }

        var disableResult =
            await _userManager
                .SetTwoFactorEnabledAsync(
                    user,
                    false);

        if (!disableResult.Succeeded)
        {
            var message =
                string.Join(
                    "; ",
                    disableResult.Errors.Select(
                        x => x.Description));

            return Result.Failure(
                Error.Validation(
                    "Identity.Validation",
                    message));
        }

        await _userManager
            .ResetAuthenticatorKeyAsync(
                user);

        user.DisableTwoFactor();

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.TwoFactorDisabled,
            ipAddress,
            userAgent,
            cancellationToken:
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<TwoFactorRecoveryCodesResponse>> GenerateRecoveryCodesAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (!user.TwoFactorEnabled)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Validation(
                    "Identity.TwoFactorNotEnabled",
                    "Xác thực hai bước chưa được bật."));
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                password);

        if (!passwordValid)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        var recoveryCodes =
            await _userManager
                .GenerateNewTwoFactorRecoveryCodesAsync(
                    user,
                    10);

        return Result.Success(
            new TwoFactorRecoveryCodesResponse(
                recoveryCodes?
                    .ToArray()
                ?? Array.Empty<string>()));
    }

    public async Task<Result<TwoFactorSetupResponse>> RegenerateAuthenticatorKeyAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null || user.IsDeleted)
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (!await _userManager
                .CheckPasswordAsync(
                    user,
                    password))
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        await _userManager
            .SetTwoFactorEnabledAsync(
                user,
                false);

        var resetResult =
            await _userManager
                .ResetAuthenticatorKeyAsync(
                    user);

        if (!resetResult.Succeeded)
        {
            var message =
                string.Join(
                    "; ",
                    resetResult.Errors.Select(
                        x => x.Description));

            return Result.Failure<TwoFactorSetupResponse>(
                Error.Validation(
                    "Identity.Validation",
                    message));
        }

        var key =
            await _userManager
                .GetAuthenticatorKeyAsync(
                    user);

        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.Validation(
                    "Identity.AuthenticatorKeyGenerationFailed",
                    "Không thể tạo khóa xác thực."));
        }

        user.DisableTwoFactor();

        var email =
            user.Email
            ?? user.UserName
            ?? user.Id.ToString();

        return Result.Success(
            new TwoFactorSetupResponse(
                FormatKey(key),
                GenerateOtpUri(
                    "HanYu",
                    email,
                    key)));
    }

    public async Task<Result<AuthResponse>> CompleteLoginAsync(
        string challengeToken,
        string code,
        LoginClientInfo client,
        CancellationToken cancellationToken = default)
    {
        var userId =
            _jwtTokenService
                .ValidateTwoFactorChallengeToken(
                    challengeToken);

        if (!userId.HasValue)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized(
                    "Identity.InvalidTwoFactorChallenge",
                    "Phiên xác thực hai bước không hợp lệ hoặc đã hết hạn."));
        }

        var user =
            await _userManager.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x => x.Id == userId.Value,
                    cancellationToken);

        if (user is null || user.IsDeleted || !user.TwoFactorEnabled)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized(
                    "Identity.InvalidTwoFactorChallenge",
                    "Phiên xác thực hai bước không hợp lệ."));
        }

        var normalizedCode =
            NormalizeAuthenticatorCode(code);

        var valid =
            await _userManager
                .VerifyTwoFactorTokenAsync(
                    user,
                    AuthenticatorProvider,
                    normalizedCode);

        if (!valid)
        {
            var recoveryResult =
                await _userManager
                    .RedeemTwoFactorRecoveryCodeAsync(
                        user,
                        code.Trim());

            valid = recoveryResult.Succeeded;
        }

        if (!valid)
        {
            await _userManager
                .AccessFailedAsync(user);

            await _securityEventService.LogAsync(
                user.Id,
                UserSecurityEventType.LoginFailed,
                client.IpAddress,
                client.UserAgent,
                new
                {
                    Reason = "InvalidTwoFactorCode"
                },
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Failure<AuthResponse>(
                Error.Unauthorized(
                    "Identity.InvalidTwoFactorCode",
                    "Mã xác thực không hợp lệ."));
        }

        await _userManager
            .ResetAccessFailedCountAsync(user);

        user.MarkLogin();

        var session =
            new UserSession(
                user.Id,
                client.DeviceName,
                client.DeviceType,
                client.Browser,
                client.OperatingSystem,
                client.IpAddress,
                client.UserAgent);

        _dbContext
            .Set<UserSession>()
            .Add(session);

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.LoginSucceeded,
            client.IpAddress,
            client.UserAgent,
            new
            {
                session.SessionKey,
                AuthenticationMethod = "TwoFactor"
            },
            cancellationToken);

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.SessionCreated,
            client.IpAddress,
            client.UserAgent,
            new
            {
                session.SessionKey
            },
            cancellationToken);

        // NOTE: SaveChanges is not called here since it's going to be called in IssueAsync,
        // Wait, AuthenticationTokenIssuer.IssueAsync calls SaveChanges! So the session and events will be saved.

        return await _tokenIssuer.IssueAsync(
            user,
            session,
            client.IpAddress,
            client.UserAgent,
            cancellationToken);
    }

    private static string GenerateOtpUri(
        string issuer,
        string email,
        string unformattedKey)
    {
        return
            $"otpauth://totp/" +
            $"{Uri.EscapeDataString(issuer)}:" +
            $"{Uri.EscapeDataString(email)}" +
            $"?secret={unformattedKey}" +
            $"&issuer={Uri.EscapeDataString(issuer)}" +
            "&digits=6";
    }

    private static string FormatKey(
        string unformattedKey)
    {
        const int chunkSize = 4;

        return string.Join(
            " ",
            Enumerable
                .Range(
                    0,
                    (unformattedKey.Length +
                     chunkSize - 1) /
                    chunkSize)
                .Select(
                    i =>
                        unformattedKey.Substring(
                            i * chunkSize,
                            Math.Min(
                                chunkSize,
                                unformattedKey.Length -
                                i * chunkSize)))
                .Select(x =>
                    x.ToLowerInvariant()));
    }

    private static string NormalizeAuthenticatorCode(
        string code)
    {
        return code
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Trim();
    }
}
