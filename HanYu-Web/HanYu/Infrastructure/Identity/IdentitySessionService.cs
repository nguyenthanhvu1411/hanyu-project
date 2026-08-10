using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Sessions;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class IdentitySessionService
    : IIdentitySessionService
{
    private readonly HanYuDbContext _dbContext;
    private readonly ISecurityEventService
        _securityEventService;

    public IdentitySessionService(
        HanYuDbContext dbContext,
        ISecurityEventService securityEventService)
    {
        _dbContext = dbContext;
        _securityEventService =
            securityEventService;
    }

    public async Task<
        Result<IReadOnlyCollection<SessionResponse>>>
        GetSessionsAsync(
            Guid userId,
            Guid? currentSessionKey,
            CancellationToken cancellationToken = default)
    {
        var sessions =
            await _dbContext
                .Set<UserSession>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .OrderByDescending(
                    x => x.LastActivityAt)
                .ToListAsync(
                    cancellationToken);

        var result =
            sessions
                .Select(
                    x => Map(
                        x,
                        currentSessionKey))
                .ToArray();

        return Result.Success<
            IReadOnlyCollection<SessionResponse>>(
            result);
    }

    public async Task<Result<SessionResponse>>
        GetCurrentSessionAsync(
            Guid userId,
            Guid sessionKey,
            CancellationToken cancellationToken = default)
    {
        var session =
            await _dbContext
                .Set<UserSession>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.SessionKey == sessionKey,
                    cancellationToken);

        if (session is null)
        {
            return Result.Failure<SessionResponse>(
                Error.NotFound(
                    "Identity.SessionNotFound",
                    "Không tìm thấy phiên đăng nhập."));
        }

        return Result.Success(
            Map(
                session,
                sessionKey));
    }

    public async Task<Result> RevokeSessionAsync(
        Guid userId,
        Guid sessionKey,
        Guid? currentSessionKey,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var session =
            await _dbContext
                .Set<UserSession>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.SessionKey == sessionKey,
                    cancellationToken);

        if (session is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.SessionNotFound",
                    "Không tìm thấy phiên đăng nhập."));
        }

        if (!session.IsActive)
        {
            return Result.Success();
        }

        session.Revoke();

        var tokens =
            await _dbContext
                .Set<RefreshToken>()
                .Where(x =>
                    x.UserId == userId &&
                    x.UserSessionId == session.Id &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke(
                ipAddress,
                "Session revoked");
        }

        await _securityEventService.LogAsync(
            userId,
            UserSecurityEventType.SessionRevoked,
            ipAddress,
            null,
            new
            {
                sessionKey,
                wasCurrentSession =
                    currentSessionKey ==
                    sessionKey
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<int>>
        RevokeAllOtherSessionsAsync(
            Guid userId,
            Guid currentSessionKey,
            string? ipAddress,
            CancellationToken cancellationToken = default)
    {
        var sessions =
            await _dbContext
                .Set<UserSession>()
                .Where(x =>
                    x.UserId == userId &&
                    x.SessionKey != currentSessionKey &&
                    x.Status ==
                        UserSessionStatus.Active &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        if (sessions.Count == 0)
        {
            return Result.Success(0);
        }

        var sessionIds =
            sessions
                .Select(x => x.Id)
                .ToArray();

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        var tokens =
            await _dbContext
                .Set<RefreshToken>()
                .Where(x =>
                    x.UserId == userId &&
                    x.UserSessionId.HasValue &&
                    sessionIds.Contains(
                        x.UserSessionId.Value) &&
                    x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke(
                ipAddress,
                "All other sessions revoked");
        }

        await _securityEventService.LogAsync(
            userId,
            UserSecurityEventType.AllSessionsRevoked,
            ipAddress,
            null,
            new
            {
                currentSessionKey,
                revokedCount = sessions.Count
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            sessions.Count);
    }

    private static SessionResponse Map(
        UserSession session,
        Guid? currentSessionKey)
    {
        return new SessionResponse(
            session.SessionKey,
            session.DeviceName,
            session.DeviceType,
            session.Browser,
            session.OperatingSystem,
            session.IpAddress,
            session.LastActivityAt,
            session.RevokedAt,
            session.Status.ToString(),
            currentSessionKey.HasValue &&
            session.SessionKey ==
                currentSessionKey.Value);
    }
}
