using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Operations.Admin.AuditLogs;

public sealed record AdminAuditLogQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public string? Action { get; init; }

    public string? EntityType { get; init; }

    public string? EntityId { get; init; }

    public string? CorrelationId { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; }
        = "-occurredAt";
}

public sealed record AdminAuditLogResponse(
    long Id,
    Guid PublicId,
    Guid? UserId,
    string Action,
    string EntityType,
    string? EntityId,
    string? EntityPublicId,
    string? OldValuesJson,
    string? NewValuesJson,
    string? ChangedPropertiesJson,
    string? IpAddress,
    string? UserAgent,
    string? CorrelationId,
    DateTimeOffset OccurredAt);
