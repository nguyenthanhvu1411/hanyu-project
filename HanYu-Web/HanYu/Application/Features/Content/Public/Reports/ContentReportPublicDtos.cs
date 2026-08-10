using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Content.Public.Reports;

public sealed record CreateContentReportRequest(
    ContentEntityType EntityType,
    long EntityId,
    ContentReportReason Reason,
    string? Description);

public sealed record MyContentReportResponse(
    Guid PublicId,
    ContentEntityType EntityType,
    ContentReportReason Reason,
    string? Description,
    ContentReportStatus Status,
    string? ResolutionNote,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
