using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Admin.Deliveries;

public sealed record AdminNotificationDeliveryQuery : PaginationRequest
{
    public long? NotificationId { get; init; }

    public NotificationChannel? Channel { get; init; }

    public NotificationDeliveryStatus? Status { get; init; }

    public string? Provider { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; } = "-createdAt";
}

public sealed record AdminNotificationDeliveryResponse(
    long Id,
    Guid PublicId,
    long NotificationId,
    Guid NotificationPublicId,
    NotificationChannel Channel,
    NotificationDeliveryStatus Status,
    string? Destination,
    string? Provider,
    string? ProviderMessageId,
    int AttemptCount,
    DateTimeOffset? LastAttemptAt,
    DateTimeOffset? SentAt,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset? FailedAt,
    string? FailureCode,
    string? FailureReason,
    DateTimeOffset CreatedAt);
