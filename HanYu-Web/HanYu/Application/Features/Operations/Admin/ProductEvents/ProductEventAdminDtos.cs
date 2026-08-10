using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Operations.Admin.ProductEvents;

public sealed record AdminProductEventQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public Guid? SessionId { get; init; }

    public string? EventName { get; init; }

    public string? EntityType { get; init; }

    public string? DeviceType { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; }
        = "-occurredAt";
}

public sealed record AdminProductEventResponse(
    long Id,
    Guid PublicId,
    Guid? UserId,
    Guid? SessionId,
    string EventName,
    string? EntityType,
    string? EntityPublicId,
    string? PropertiesJson,
    string? PagePath,
    string? Referrer,
    string? DeviceType,
    DateTimeOffset OccurredAt);
