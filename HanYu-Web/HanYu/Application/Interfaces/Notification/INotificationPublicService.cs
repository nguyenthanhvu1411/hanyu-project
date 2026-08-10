using HanYu.Application.Common.Models;
using HanYu.Application.Features.Notification.Public.Notifications;
using HanYu.Application.Features.Notification.Public.Preferences;

namespace HanYu.Application.Interfaces.Notification;

public interface INotificationPublicService
{
    Task<Result<PagedResult<NotificationResponse>>> GetMyNotificationsAsync(
        Guid userId,
        NotificationQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<int>> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result> MarkReadAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<Result> MarkUnreadAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<Result> MarkAllReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationPreferenceResponse>> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationPreferenceResponse>> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferenceRequest request,
        CancellationToken cancellationToken = default);
}
