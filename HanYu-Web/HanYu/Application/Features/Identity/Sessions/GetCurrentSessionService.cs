using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Sessions;

public sealed class GetCurrentSessionService
{
    private readonly IIdentitySessionService
        _sessionService;

    public GetCurrentSessionService(
        IIdentitySessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Task<Result<SessionResponse>> ExecuteAsync(
        Guid userId,
        Guid sessionKey,
        CancellationToken cancellationToken = default)
    {
        return _sessionService
            .GetCurrentSessionAsync(
                userId,
                sessionKey,
                cancellationToken);
    }
}
