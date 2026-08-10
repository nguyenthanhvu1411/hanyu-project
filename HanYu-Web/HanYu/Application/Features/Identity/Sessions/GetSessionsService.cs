using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Sessions;

public sealed class GetSessionsService
{
    private readonly IIdentitySessionService
        _sessionService;

    public GetSessionsService(
        IIdentitySessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Task<
        Result<IReadOnlyCollection<SessionResponse>>>
        ExecuteAsync(
            Guid userId,
            Guid? currentSessionKey,
            CancellationToken cancellationToken = default)
    {
        return _sessionService.GetSessionsAsync(
            userId,
            currentSessionKey,
            cancellationToken);
    }
}
