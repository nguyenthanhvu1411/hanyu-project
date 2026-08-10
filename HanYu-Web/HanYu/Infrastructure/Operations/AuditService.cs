using HanYu.Application.Interfaces.Operations;
using Microsoft.Extensions.Logging;

namespace HanYu.Infrastructure.Operations;

public sealed class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(string action, string targetId, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Audit Log: Action={Action}, Target={TargetId}, Reason={Reason}", action, targetId, reason);
        return Task.CompletedTask;
    }
}
