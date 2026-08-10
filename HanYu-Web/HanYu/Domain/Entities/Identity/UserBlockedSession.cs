using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Identity;

public class UserBlockedSession : BaseEntity
{
    public Guid UserId { get; private set; }

    public long UserSessionId { get; private set; }

    public string Reason { get; private set; }
        = string.Empty;

    public DateTimeOffset BlockedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; private set; }

    public Guid? BlockedByUserId { get; private set; }

    public string? IpAddress { get; private set; }

    public User User { get; private set; } = null!;

    public UserSession UserSession { get; private set; } = null!;

    public bool IsExpired =>
        ExpiresAt.HasValue &&
        ExpiresAt.Value <= DateTimeOffset.UtcNow;

    public bool IsActive =>
        !IsExpired;

    protected UserBlockedSession()
    {
    }

    public UserBlockedSession(
        Guid userId,
        long userSessionId,
        string reason,
        Guid? blockedByUserId = null,
        DateTimeOffset? expiresAt = null,
        string? ipAddress = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (userSessionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(userSessionId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Lý do block session không được để trống.",
                nameof(reason));

        if (blockedByUserId.HasValue &&
            blockedByUserId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "BlockedByUserId không hợp lệ.",
                nameof(blockedByUserId));
        }

        if (expiresAt.HasValue &&
            expiresAt.Value <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAt),
                "ExpiresAt phải ở tương lai.");
        }

        UserId = userId;
        UserSessionId = userSessionId;

        Reason = reason.Trim();

        BlockedByUserId =
            blockedByUserId;

        ExpiresAt =
            expiresAt;

        IpAddress =
            Normalize(ipAddress);
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
