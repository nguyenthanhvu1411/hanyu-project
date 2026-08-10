using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.SecurityEvents;
using HanYu.Domain.Enums;

namespace HanYu.Application.Interfaces.Authentication;

public interface ISecurityEventService
{
    Task LogAsync(
        Guid userId,
        UserSecurityEventType eventType,
        string? ipAddress = null,
        string? userAgent = null,
        object? metadata = null,
        CancellationToken cancellationToken = default);

    Task<
        Result<IReadOnlyCollection<SecurityEventResponse>>>
        GetAsync(
            Guid userId,
            int take = 50,
            CancellationToken cancellationToken = default);
}
