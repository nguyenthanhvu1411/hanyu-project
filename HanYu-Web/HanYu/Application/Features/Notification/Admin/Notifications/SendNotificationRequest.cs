using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Admin.Notifications;

public sealed record SendNotificationRequest(
    Guid UserId,
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl,
    string? MetadataJson,
    DateTimeOffset? ExpiresAt);
