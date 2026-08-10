using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Identity.SecurityEvents;

public sealed class LogSecurityEventService
{
    private readonly ISecurityEventService
        _securityEventService;

    public LogSecurityEventService(
        ISecurityEventService securityEventService)
    {
        _securityEventService =
            securityEventService;
    }

    public Task ExecuteAsync(
        Guid userId,
        UserSecurityEventType eventType,
        string? ipAddress = null,
        string? userAgent = null,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return _securityEventService.LogAsync(
            userId,
            eventType,
            ipAddress,
            userAgent,
            metadata,
            cancellationToken);
    }
}
