using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.RevokeSession;

public sealed class RevokeAllOtherSessionsService
{
    private readonly IIdentitySessionService
        _sessionService;

    public RevokeAllOtherSessionsService(
        IIdentitySessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Task<Result<int>> ExecuteAsync(
        Guid userId,
        Guid currentSessionKey,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        return _sessionService
            .RevokeAllOtherSessionsAsync(
                userId,
                currentSessionKey,
                ipAddress,
                cancellationToken);
    }
}
