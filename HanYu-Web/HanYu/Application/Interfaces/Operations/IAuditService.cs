namespace HanYu.Application.Interfaces.Operations;

public interface IAuditService
{
    Task WriteAsync(string action, string targetId, string reason, CancellationToken cancellationToken = default);
}
