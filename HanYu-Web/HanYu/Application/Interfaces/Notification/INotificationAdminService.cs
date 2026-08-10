using HanYu.Application.Common.Models;
using HanYu.Application.Features.Notification.Admin.Deliveries;
using HanYu.Application.Features.Notification.Admin.Notifications;

namespace HanYu.Application.Interfaces.Notification;

public interface INotificationAdminService
{
    Task<Result<PagedResult<AdminNotificationResponse>>> GetNotificationsAsync(
        AdminNotificationQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<AdminNotificationResponse>> GetNotificationAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<AdminNotificationResponse>> SendAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<int>> BroadcastAsync(
        BroadcastNotificationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminNotificationDeliveryResponse>>> GetDeliveriesAsync(
        AdminNotificationDeliveryQuery query,
        CancellationToken cancellationToken = default);

    Task<Result> RetryDeliveryAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> CancelDeliveryAsync(
        long id,
        CancellationToken cancellationToken = default);
}
