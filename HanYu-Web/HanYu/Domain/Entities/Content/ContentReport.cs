using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Content;

public class ContentReport : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public ContentEntityType EntityType { get; private set; }

    public long EntityId { get; private set; }

    public ContentReportReason Reason { get; private set; }

    public string? Description { get; private set; }

    public ContentReportStatus Status { get; private set; }
        = ContentReportStatus.Open;

    public Guid? ResolvedByUserId { get; private set; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public string? ResolutionNote { get; private set; }

    protected ContentReport()
    {
    }

    public ContentReport(
        Guid userId,
        ContentEntityType entityType,
        long entityId,
        ContentReportReason reason,
        string? description = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (entityId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(entityId));

        UserId = userId;
        EntityType = entityType;
        EntityId = entityId;
        Reason = reason;
        Description = Normalize(description);
    }

    public void UpdateDescription(
        string? description)
    {
        EnsureOpenOrInReview();

        Description = Normalize(description);

        MarkUpdated();
    }

    public void StartReview()
    {
        if (Status != ContentReportStatus.Open)
            throw new InvalidOperationException(
                "Chỉ report Open mới có thể chuyển sang InReview.");

        Status = ContentReportStatus.InReview;

        MarkUpdated();
    }

    public void Resolve(
        Guid resolvedByUserId,
        string? resolutionNote = null)
    {
        EnsureResolver(resolvedByUserId);

        if (Status is ContentReportStatus.Resolved or
            ContentReportStatus.Rejected)
        {
            return;
        }

        ResolvedByUserId = resolvedByUserId;
        ResolutionNote = Normalize(resolutionNote);
        ResolvedAt = DateTimeOffset.UtcNow;

        Status = ContentReportStatus.Resolved;

        MarkUpdated();
    }

    public void Reject(
        Guid resolvedByUserId,
        string? resolutionNote = null)
    {
        EnsureResolver(resolvedByUserId);

        if (Status is ContentReportStatus.Resolved or
            ContentReportStatus.Rejected)
        {
            return;
        }

        ResolvedByUserId = resolvedByUserId;
        ResolutionNote = Normalize(resolutionNote);
        ResolvedAt = DateTimeOffset.UtcNow;

        Status = ContentReportStatus.Rejected;

        MarkUpdated();
    }

    public void Reopen()
    {
        if (Status == ContentReportStatus.Open)
            return;

        Status = ContentReportStatus.Open;
        ResolvedByUserId = null;
        ResolvedAt = null;
        ResolutionNote = null;

        MarkUpdated();
    }

    private void EnsureOpenOrInReview()
    {
        if (Status is not ContentReportStatus.Open and
            not ContentReportStatus.InReview)
        {
            throw new InvalidOperationException(
                "Report đã đóng không thể cập nhật.");
        }
    }

    private static void EnsureResolver(
        Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "ResolvedByUserId không hợp lệ.",
                nameof(userId));
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}