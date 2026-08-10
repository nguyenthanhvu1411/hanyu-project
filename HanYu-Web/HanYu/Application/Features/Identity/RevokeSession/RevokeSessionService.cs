using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.RevokeSession;

public sealed class RevokeSessionService
{
    private readonly IIdentitySessionService
        _sessionService;

    public RevokeSessionService(
        IIdentitySessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Task<Result> ExecuteAsync(
        Guid userId,
        Guid sessionKey,
        Guid? currentSessionKey,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        return _sessionService.RevokeSessionAsync(
            userId,
            sessionKey,
            currentSessionKey,
            ipAddress,
            cancellationToken);
    }
}
