using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.SecurityEvents;

public sealed class GetSecurityEventsService
{
    private readonly ISecurityEventService
        _securityEventService;

    public GetSecurityEventsService(
        ISecurityEventService securityEventService)
    {
        _securityEventService =
            securityEventService;
    }

    public Task<
        Result<IReadOnlyCollection<SecurityEventResponse>>>
        ExecuteAsync(
            Guid userId,
            int take = 50,
            CancellationToken cancellationToken = default)
    {
        return _securityEventService.GetAsync(
            userId,
            take,
            cancellationToken);
    }
}
