using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using HanYu.Domain.Constants;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly HanYuDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly ISecurityEventService _securityEventService;
    private readonly IAuthenticationTokenIssuer _tokenIssuer;

    public IdentityService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        HanYuDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions,
        ISecurityEventService securityEventService,
        IAuthenticationTokenIssuer tokenIssuer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
        _securityEventService = securityEventService;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var userName = request.UserName.Trim();

        var existingEmail =
            await _userManager.FindByEmailAsync(email);

        if (existingEmail is not null)
        {
            return Result.Failure<AuthResponse>(
                Error.Conflict(
                    "Identity.EmailAlreadyExists",
                    "Email đã được sử dụng."));
        }

        var existingUserName =
            await _userManager.FindByNameAsync(userName);

        if (existingUserName is not null)
        {
            return Result.Failure<AuthResponse>(
                Error.Conflict(
                    "Identity.UserNameAlreadyExists",
                    "Username đã được sử dụng."));
        }

        var user = new User(
            userName,
            email);

        var createResult =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!createResult.Succeeded)
        {
            return Result.Failure<AuthResponse>(
                CreateIdentityError(createResult));
        }

        try
        {
            await EnsureDefaultUserRoleAsync();

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    Roles.User);

            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        addRoleResult.Errors
                            .Select(x => x.Description)));
            }

            var profile =
                new UserProfile(
                    user.Id,
                    request.DisplayName);

            var preference =
                new UserPreference(user.Id);

            _dbContext.Set<UserProfile>().Add(profile);
            _dbContext.Set<UserPreference>().Add(preference);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            var session = new UserSession(
                user.Id,
                deviceName: "Registration",
                deviceType: null,
                browser: null,
                operatingSystem: null,
                ipAddress: null,
                userAgent: null);

            return await _tokenIssuer.IssueAsync(
                user,
                session,
                null,
                null,
                cancellationToken);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x => x.NormalizedEmail ==
                         email.ToUpperInvariant(),
                    cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return InvalidCredentials();
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return Result.Failure<AuthResponse>(
                Error.Forbidden(
                    "Identity.AccountLocked",
                    "Tài khoản đang tạm thời bị khóa."));
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);

            _dbContext
                .Set<UserLoginHistory>()
                .Add(
                    new UserLoginHistory(
                        user.Id,
                        false,
                        request.IpAddress,
                        request.UserAgent,
                        request.DeviceName,
                        request.Browser,
                        request.OperatingSystem,
                        "Invalid password"));

            await _securityEventService.LogAsync(
                user.Id,
                UserSecurityEventType.LoginFailed,
                request.IpAddress,
                request.UserAgent,
                new
                {
                    request.DeviceName,
                    request.DeviceType,
                    request.Browser,
                    request.OperatingSystem,
                    Reason = "InvalidPassword"
                },
                cancellationToken);

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _securityEventService.LogAsync(
                    user.Id,
                    UserSecurityEventType.AccountLocked,
                    request.IpAddress,
                    request.UserAgent,
                    new
                    {
                        Reason = "MaximumFailedAccessAttempts"
                    },
                    cancellationToken);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return InvalidCredentials();
        }

        if (user.TwoFactorEnabled)
        {
            _dbContext
                .Set<UserLoginHistory>()
                .Add(
                    new UserLoginHistory(
                        user.Id,
                        true,
                        request.IpAddress,
                        request.UserAgent,
                        request.DeviceName,
                        request.Browser,
                        request.OperatingSystem));

            await _dbContext.SaveChangesAsync(cancellationToken);

            var challenge =
                _jwtTokenService.GenerateTwoFactorChallengeToken(
                    user.Id,
                    user.Email ?? string.Empty);

            return Result.Success(
                AuthResponse.TwoFactorRequired(
                    challenge.Token,
                    challenge.ExpiresAtUtc));
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        user.MarkLogin();

        var session =
            new UserSession(
                user.Id,
                request.DeviceName,
                request.DeviceType,
                request.Browser,
                request.OperatingSystem,
                request.IpAddress,
                request.UserAgent);

        _dbContext.Set<UserSession>().Add(session);

        _dbContext
            .Set<UserLoginHistory>()
            .Add(
                new UserLoginHistory(
                    user.Id,
                    true,
                    request.IpAddress,
                    request.UserAgent,
                    request.DeviceName,
                    request.Browser,
                    request.OperatingSystem));

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.LoginSucceeded,
            request.IpAddress,
            request.UserAgent,
            new
            {
                session.SessionKey,
                request.DeviceName,
                request.DeviceType,
                request.Browser,
                request.OperatingSystem
            },
            cancellationToken);

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.SessionCreated,
            request.IpAddress,
            request.UserAgent,
            new
            {
                session.SessionKey,
                request.DeviceName,
                request.DeviceType
            },
            cancellationToken);

        // No need to SaveChanges here because _tokenIssuer.IssueAsync will save it.
        // Wait, UserLoginHistory needs to be saved, and session is added but not saved.
        // Actually, _tokenIssuer.IssueAsync calls SaveChangesAsync, so it's fine to rely on it.

        return await _tokenIssuer.IssueAsync(
            user,
            session,
            request.IpAddress,
            request.UserAgent,
            cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshIdentityTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return InvalidRefreshToken();
        }

        var hash =
            HashRefreshToken(
                request.RefreshToken);

        var currentToken =
            await _dbContext
                .Set<RefreshToken>()
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.UserSession)
                .FirstOrDefaultAsync(
                    x => x.TokenHash == hash,
                    cancellationToken);

        if (currentToken is null)
        {
            return InvalidRefreshToken();
        }

        if (currentToken.IsUsed)
        {
            await HandleRefreshTokenReuseAsync(
                currentToken,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);

            return InvalidRefreshToken();
        }

        if (currentToken.IsRevoked ||
            currentToken.IsExpired)
        {
            return InvalidRefreshToken();
        }

        if (currentToken.User.IsDeleted)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized(
                    "Identity.UserDeleted",
                    "Tài khoản không còn hoạt động."));
        }

        if (currentToken.UserSession is not null &&
            !currentToken.UserSession.IsActive)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized(
                    "Identity.SessionRevoked",
                    "Phiên đăng nhập không còn hoạt động."));
        }

        var rawNewRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var newToken =
            new RefreshToken(
                currentToken.UserId,
                currentToken.UserSessionId,
                HashRefreshToken(rawNewRefreshToken),
                DateTimeOffset.UtcNow.AddDays(
                    _jwtOptions.RefreshTokenExpirationDays),
                request.IpAddress,
                request.UserAgent,
                currentToken.FamilyId);

        currentToken.MarkUsed();

        _dbContext
            .Set<RefreshToken>()
            .Add(newToken);

        currentToken.UserSession?.Touch();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var session = currentToken.UserSession ?? new UserSession(
            currentToken.User.Id,
            null,
            null,
            null,
            null,
            null,
            null);
            
        if (currentToken.UserSession is null)
        {
            _dbContext.Set<UserSession>().Add(session);
        }

        return await _tokenIssuer.IssueAsync(
            currentToken.User,
            session,
            request.IpAddress,
            request.UserAgent,
            cancellationToken);
    }

    public async Task<Result> LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Success();
        }

        var hash =
            HashRefreshToken(refreshToken);

        var token =
            await _dbContext
                .Set<RefreshToken>()
                .Include(x => x.UserSession)
                .FirstOrDefaultAsync(
                    x => x.TokenHash == hash,
                    cancellationToken);

        if (token is null)
        {
            return Result.Success();
        }

        token.Revoke(
            ipAddress,
            "User logout");

        token.UserSession?.Revoke();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CurrentUserResponse>>
        GetCurrentUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return Result.Failure<CurrentUserResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        return Result.Success(
            new CurrentUserResponse(
                user.Id,
                user.PublicId,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                user.EmailConfirmed,
                user.Profile?.DisplayName,
                user.Profile?.AvatarUrl,
                roles.ToArray()));
    }

    private async Task EnsureDefaultUserRoleAsync()
    {
        if (await _roleManager.RoleExistsAsync(
                Roles.User))
        {
            return;
        }

        var role =
            new Role(
                Roles.User,
                "Default application user");

        var result =
            await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    result.Errors
                        .Select(x => x.Description)));
        }
    }

    private static string HashRefreshToken(
        string token)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }

    private async Task HandleRefreshTokenReuseAsync(
        RefreshToken reusedToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var familyTokens =
            await _dbContext
                .Set<RefreshToken>()
                .Where(x =>
                    x.UserId == reusedToken.UserId &&
                    x.FamilyId == reusedToken.FamilyId &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in familyTokens)
        {
            token.Revoke(
                ipAddress,
                "Refresh token reuse detected");
        }

        UserSession? session = null;

        if (reusedToken.UserSessionId.HasValue)
        {
            session =
                await _dbContext
                    .Set<UserSession>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            reusedToken.UserSessionId.Value,
                        cancellationToken);

            if (session is not null &&
                session.IsActive)
            {
                session.Revoke();
            }
        }

        await _securityEventService.LogAsync(
            reusedToken.UserId,
            UserSecurityEventType.RefreshTokenReuseDetected,
            ipAddress,
            userAgent,
            new
            {
                reusedToken.FamilyId,
                SessionKey = session?.SessionKey,
                FamilyTokenCount = familyTokens.Count
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static Result<AuthResponse>
        InvalidCredentials()
    {
        return Result.Failure<AuthResponse>(
            Error.Unauthorized(
                "Identity.InvalidCredentials",
                "Email hoặc mật khẩu không chính xác."));
    }

    private static Result<AuthResponse>
        InvalidRefreshToken()
    {
        return Result.Failure<AuthResponse>(
            Error.Unauthorized(
                "Identity.InvalidRefreshToken",
                "Refresh token không hợp lệ hoặc đã hết hạn."));
    }

    private static Error CreateIdentityError(
        IdentityResult result)
    {
        var message =
            string.Join(
                "; ",
                result.Errors.Select(
                    x => x.Description));

        return Error.Validation(
            "Identity.Validation",
            message);
    }

    public async Task<Result<string>>
        GenerateEmailVerificationTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null ||
            user.IsDeleted)
        {
            return Result.Failure<string>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure<string>(
                Error.Conflict(
                    "Identity.EmailAlreadyVerified",
                    "Email đã được xác minh."));
        }

        var token =
            await _userManager
                .GenerateEmailConfirmationTokenAsync(
                    user);

        var encodedToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

        return Result.Success(
            encodedToken);
    }

    public async Task<Result> VerifyEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidEmailVerificationToken",
                    "Token xác minh email không hợp lệ."));
        }

        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null ||
            user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        string decodedToken;

        try
        {
            var tokenBytes =
                WebEncoders.Base64UrlDecode(token);

            decodedToken =
                Encoding.UTF8.GetString(
                    tokenBytes);
        }
        catch
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidEmailVerificationToken",
                    "Token xác minh email không hợp lệ."));
        }

        var result =
            await _userManager.ConfirmEmailAsync(
                user,
                decodedToken);

        if (!result.Succeeded)
        {
            return Result.Failure(
                CreateIdentityError(result));
        }

        user.ConfirmEmail();

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.EmailVerified,
            metadata: new
            {
                Email = user.Email
            },
            cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<string?>>
        GeneratePasswordResetTokenAsync(
            string email,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<string?>(
                Error.Validation(
                    "Identity.InvalidEmail",
                    "Email không hợp lệ."));
        }

        email =
            email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager.FindByEmailAsync(
                email);

        // Không tiết lộ email có tồn tại.
        if (user is null ||
            user.IsDeleted)
        {
            return Result.Success<string?>(
                null);
        }

        var token =
            await _userManager
                .GeneratePasswordResetTokenAsync(
                    user);

        var encodedToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

        return Result.Success<string?>(
            encodedToken);
    }

    public async Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidEmail",
                    "Email không hợp lệ."));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidResetToken",
                    "Reset token không hợp lệ."));
        }

        email =
            email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager.FindByEmailAsync(
                email);

        if (user is null ||
            user.IsDeleted)
        {
            // Không tiết lộ tài khoản tồn tại.
            return Result.Success();
        }

        string decodedToken;

        try
        {
            var bytes =
                WebEncoders.Base64UrlDecode(token);

            decodedToken =
                Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return Result.Failure(
                Error.Validation(
                    "Identity.InvalidResetToken",
                    "Reset token không hợp lệ."));
        }

        var result =
            await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                newPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(
                CreateIdentityError(result));
        }

        // Password reset là security-sensitive action.
        // Revoke toàn bộ session đang hoạt động.

        var sessions =
            await _dbContext
                .Set<UserSession>()
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        var refreshTokens =
            await _dbContext
                .Set<RefreshToken>()
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(
                null,
                "Password reset");
        }

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.PasswordReset,
            metadata: new
            {
                RevokedSessions = sessions.Count,
                RevokedRefreshTokens = refreshTokens.Count
            },
            cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null ||
            user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var result =
            await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(
                CreateIdentityError(result));
        }

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.PasswordChanged,
            metadata: new
            {
                ChangedAt = DateTimeOffset.UtcNow
            },
            cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IdentityEmailUser?>>
        FindUserForEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Success<IdentityEmailUser?>(
                null);
        }

        email =
            email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x =>
                        x.NormalizedEmail ==
                        email.ToUpperInvariant(),
                    cancellationToken);

        if (user is null ||
            user.IsDeleted)
        {
            return Result.Success<IdentityEmailUser?>(
                null);
        }

        return Result.Success<IdentityEmailUser?>(
            new IdentityEmailUser(
                user.Id,
                user.Email ?? email,
                user.Profile?.DisplayName
                    ?? user.UserName
                    ?? "HanYu User"));
    }
}