namespace HanYu.Application.Features.Operations.Public.Events;

public sealed record TrackProductEventRequest(
    Guid? SessionId,
    string EventName,
    string? EntityType,
    string? EntityPublicId,
    string? PropertiesJson,
    string? PagePath,
    string? Referrer,
    string? DeviceType);
