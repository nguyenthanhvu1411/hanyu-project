using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Admin.Notifications;

public sealed record BroadcastNotificationRequest(
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl,
    string? MetadataJson,
    DateTimeOffset? ExpiresAt,
    IReadOnlyCollection<Guid>? UserIds);
