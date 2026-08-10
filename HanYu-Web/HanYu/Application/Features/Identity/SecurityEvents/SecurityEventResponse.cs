namespace HanYu.Application.Features.Identity.SecurityEvents;

public sealed record SecurityEventResponse(
    string EventType,
    string? IpAddress,
    string? UserAgent,
    string? MetadataJson,
    DateTimeOffset OccurredAt);
