using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Sessions;

namespace HanYu.Application.Interfaces.Authentication;

public interface IIdentitySessionService
{
    Task<Result<IReadOnlyCollection<SessionResponse>>>
        GetSessionsAsync(
            Guid userId,
            Guid? currentSessionKey,
            CancellationToken cancellationToken = default);

    Task<Result<SessionResponse>>
        GetCurrentSessionAsync(
            Guid userId,
            Guid sessionKey,
            CancellationToken cancellationToken = default);

    Task<Result> RevokeSessionAsync(
        Guid userId,
        Guid sessionKey,
        Guid? currentSessionKey,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<int>> RevokeAllOtherSessionsAsync(
        Guid userId,
        Guid currentSessionKey,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
