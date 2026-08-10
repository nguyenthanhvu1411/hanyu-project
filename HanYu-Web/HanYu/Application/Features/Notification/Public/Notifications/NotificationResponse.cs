using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Public.Notifications;

public sealed record NotificationResponse(
    Guid PublicId,
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
