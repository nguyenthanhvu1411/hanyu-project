using HanYu.Application.Interfaces.Authentication;
using Microsoft.Extensions.Logging;

namespace HanYu.Infrastructure.Authentication;

public sealed class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;

    public SessionService(ILogger<SessionService> logger)
    {
        _logger = logger;
    }

    public Task RevokeAllAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoked all sessions for user {UserId}. Reason: {Reason}", userId, reason);
        return Task.CompletedTask;
    }
}
