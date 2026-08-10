using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Notification;

public class NotificationDelivery : BaseEntity
{
    public long NotificationId { get; private set; }

    public NotificationChannel Channel { get; private set; }

    public NotificationDeliveryStatus Status { get; private set; }
        = NotificationDeliveryStatus.Pending;

    public string? Destination { get; private set; }

    public string? Provider { get; private set; }

    public string? ProviderMessageId { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? LastAttemptAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public DateTimeOffset? DeliveredAt { get; private set; }

    public DateTimeOffset? FailedAt { get; private set; }

    public string? FailureCode { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public InAppNotification Notification { get; private set; }
        = null!;

    protected NotificationDelivery()
    {
    }

    public NotificationDelivery(
        long notificationId,
        NotificationChannel channel,
        string? destination = null,
        string? provider = null)
    {
        if (notificationId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(notificationId));

        NotificationId = notificationId;
        Channel = channel;
        Destination = Normalize(destination);
        Provider = Normalize(provider);
    }

    public void StartAttempt()
    {
        if (Status is NotificationDeliveryStatus.Delivered or
            NotificationDeliveryStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Delivery đã kết thúc.");
        }

        AttemptCount++;

        LastAttemptAt =
            DateTimeOffset.UtcNow;

        Status =
            NotificationDeliveryStatus.Processing;

        FailureCode = null;
        FailureReason = null;
        FailedAt = null;
    }

    public void MarkSent(
        string? providerMessageId = null)
    {
        if (Status != NotificationDeliveryStatus.Processing)
            throw new InvalidOperationException(
                "Delivery phải đang Processing.");

        Status =
            NotificationDeliveryStatus.Sent;

        ProviderMessageId =
            Normalize(providerMessageId);

        SentAt =
            DateTimeOffset.UtcNow;
    }

    public void MarkDelivered()
    {
        if (Status is not NotificationDeliveryStatus.Sent and
            not NotificationDeliveryStatus.Processing)
        {
            throw new InvalidOperationException(
                "Delivery chưa ở trạng thái có thể Delivered.");
        }

        Status =
            NotificationDeliveryStatus.Delivered;

        SentAt ??=
            DateTimeOffset.UtcNow;

        DeliveredAt =
            DateTimeOffset.UtcNow;

        FailureCode = null;
        FailureReason = null;
        FailedAt = null;
    }

    public void MarkFailed(
        string? failureCode,
        string failureReason)
    {
        if (Status == NotificationDeliveryStatus.Delivered)
            throw new InvalidOperationException(
                "Delivery đã Delivered.");

        if (string.IsNullOrWhiteSpace(failureReason))
            throw new ArgumentException(
                "FailureReason không được để trống.",
                nameof(failureReason));

        Status =
            NotificationDeliveryStatus.Failed;

        FailureCode =
            Normalize(failureCode);

        FailureReason =
            failureReason.Trim();

        FailedAt =
            DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status is NotificationDeliveryStatus.Delivered or
            NotificationDeliveryStatus.Cancelled)
        {
            return;
        }

        Status =
            NotificationDeliveryStatus.Cancelled;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
