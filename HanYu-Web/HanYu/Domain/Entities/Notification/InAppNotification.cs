using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Notification;

public class InAppNotification : BaseEntity
{
    public Guid UserId { get; private set; }

    public NotificationType Type { get; private set; }

    public string Title { get; private set; }
        = string.Empty;

    public string Message { get; private set; }
        = string.Empty;

    public string? ActionUrl { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? ReadAt { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public bool IsRead => ReadAt.HasValue;

    public bool IsExpired =>
        ExpiresAt.HasValue &&
        ExpiresAt.Value <= DateTimeOffset.UtcNow;

    protected InAppNotification()
    {
    }

    public InAppNotification(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? metadataJson = null,
        DateTimeOffset? expiresAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Title không được để trống.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Message không được để trống.",
                nameof(message));

        title = title.Trim();
        message = message.Trim();

        if (title.Length > 200)
            throw new ArgumentException(
                "Title không được vượt quá 200 ký tự.",
                nameof(title));

        if (expiresAt.HasValue &&
            expiresAt.Value <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAt),
                "ExpiresAt phải ở tương lai.");
        }

        UserId = userId;
        Type = type;
        Title = title;
        Message = message;

        ActionUrl = Normalize(actionUrl);
        MetadataJson = NormalizeJson(metadataJson);
        ExpiresAt = expiresAt;
    }

    public void MarkRead()
    {
        if (IsRead)
            return;

        if (IsExpired)
            throw new InvalidOperationException(
                "Notification đã hết hạn.");

        ReadAt = DateTimeOffset.UtcNow;
    }

    public void MarkUnread()
    {
        if (!IsRead)
            return;

        ReadAt = null;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static string? NormalizeJson(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
