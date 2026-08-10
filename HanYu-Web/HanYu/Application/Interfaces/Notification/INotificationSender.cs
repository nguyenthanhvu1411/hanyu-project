using HanYu.Domain.Enums;

namespace HanYu.Application.Interfaces.Notification;

public interface INotificationSender
{
    Task SendAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? metadataJson = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken cancellationToken = default);
}
