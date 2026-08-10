using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Account;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Email;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly HanYuDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ISecurityEventService _securityEventService;

    public AccountService(
        UserManager<User> userManager,
        HanYuDbContext dbContext,
        IIdentityService identityService,
        IEmailService emailService,
        ISecurityEventService securityEventService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _identityService = identityService;
        _emailService = emailService;
        _securityEventService = securityEventService;
    }

    public async Task<Result<AccountResponse>> ChangeEmailAsync(
        Guid userId,
        ChangeEmailRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(userId);

        if (user is null || user.IsDeleted)
            return UserNotFound<AccountResponse>();

        if (!await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            return Result.Failure<AccountResponse>(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        var newEmail =
            request.NewEmail
                .Trim()
                .ToLowerInvariant();

        var existing =
            await _userManager.FindByEmailAsync(
                newEmail);

        if (existing is not null &&
            existing.Id != user.Id)
        {
            return Result.Failure<AccountResponse>(
                Error.Conflict(
                    "Identity.EmailAlreadyExists",
                    "Email đã được sử dụng."));
        }

        try
        {
            user.UpdateEmail(newEmail);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<AccountResponse>(
                Error.Validation(
                    "Identity.InvalidEmail",
                    exception.Message));
        }

        var updateResult =
            await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result.Failure<AccountResponse>(
                ToIdentityError(updateResult));
        }

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.EmailChanged,
            ipAddress,
            userAgent,
            new
            {
                NewEmail = newEmail
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        // Gửi verification tới email mới.
        var tokenResult =
            await _identityService
                .GenerateEmailVerificationTokenAsync(
                    user.Id,
                    cancellationToken);

        if (tokenResult.IsSuccess)
        {
            await _emailService.SendVerificationEmailAsync(
                user.Email!,
                user.Profile?.DisplayName
                    ?? user.UserName
                    ?? "HanYu User",
                user.Id,
                tokenResult.Value,
                cancellationToken);
        }

        return Result.Success(Map(user));
    }

    public async Task<Result<AccountResponse>>
        ChangeUsernameAsync(
            Guid userId,
            ChangeUsernameRequest request,
            CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(userId);

        if (user is null || user.IsDeleted)
            return UserNotFound<AccountResponse>();

        if (!await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            return Result.Failure<AccountResponse>(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        var existing =
            await _userManager.FindByNameAsync(
                request.NewUserName.Trim());

        if (existing is not null &&
            existing.Id != user.Id)
        {
            return Result.Failure<AccountResponse>(
                Error.Conflict(
                    "Identity.UsernameAlreadyExists",
                    "Username đã được sử dụng."));
        }

        try
        {
            user.UpdateUserName(
                request.NewUserName);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<AccountResponse>(
                Error.Validation(
                    "Identity.InvalidUsername",
                    exception.Message));
        }

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<AccountResponse>(
                ToIdentityError(result));
        }

        return Result.Success(Map(user));
    }

    public async Task<Result> DeleteAsync(
        Guid userId,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (!await _userManager.CheckPasswordAsync(
                user,
                password))
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Identity.InvalidPassword",
                    "Mật khẩu không chính xác."));
        }

        var sessions =
            await _dbContext
                .Set<UserSession>()
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedAt == null)
                .ToListAsync(cancellationToken);

        foreach (var session in sessions)
            session.Revoke();

        var refreshTokens =
            await _dbContext
                .Set<RefreshToken>()
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedAt == null)
                .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.Revoke(
                ipAddress,
                "Account deleted");
        }

        user.SoftDelete();

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.AccountDeleted,
            ipAddress,
            userAgent,
            cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RestoreAsync(
        Guid publicId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .FirstOrDefaultAsync(
                    x => x.PublicId == publicId,
                    cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (!user.IsDeleted)
            return Result.Success();

        user.Restore();

        await _securityEventService.LogAsync(
            user.Id,
            UserSecurityEventType.AccountRestored,
            ipAddress,
            userAgent,
            cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private Task<User?> GetUserAsync(Guid userId)
    {
        return _userManager.Users
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(
                x => x.Id == userId);
    }

    private static AccountResponse Map(User user)
    {
        return new AccountResponse(
            user.PublicId,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.CreatedAt,
            user.LastLoginAt);
    }

    private static Result<T> UserNotFound<T>()
    {
        return Result.Failure<T>(
            Error.NotFound(
                "Identity.UserNotFound",
                "Không tìm thấy người dùng."));
    }

    private static Error ToIdentityError(
        IdentityResult result)
    {
        var message =
            string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

        return Error.Validation(
            "Identity.ValidationFailed",
            message);
    }
}
