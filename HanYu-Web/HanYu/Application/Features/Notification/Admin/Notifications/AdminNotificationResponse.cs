using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Admin.Notifications;

public sealed record AdminNotificationResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl,
    string? MetadataJson,
    bool IsRead,
    bool IsExpired,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReadAt,
    DateTimeOffset? ExpiresAt);
