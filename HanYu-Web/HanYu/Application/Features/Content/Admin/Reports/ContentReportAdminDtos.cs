using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Content.Admin.Reports;

public sealed record AdminContentReportQuery : PaginationRequest
{
    public Guid? UserId { get; init; }
    public ContentEntityType? EntityType { get; init; }
    public ContentReportReason? Reason { get; init; }
    public ContentReportStatus? Status { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Sort { get; init; } = "-createdAt";
}

public sealed record AdminContentReportResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    ContentEntityType EntityType,
    long EntityId,
    ContentReportReason Reason,
    string? Description,
    ContentReportStatus Status,
    Guid? ResolvedByUserId,
    DateTimeOffset? ResolvedAt,
    string? ResolutionNote,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? UserDisplayName = null,
    string? UserEmail = null,
    string? EntityDisplayName = null,
    string? ResolvedByDisplayName = null);

public sealed record ResolveContentReportRequest(string? ResolutionNote);
