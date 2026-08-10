using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Identity;

public class UserSession : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public Guid SessionKey { get; private set; }
        = Guid.NewGuid();

    public string? DeviceName { get; private set; }

    public string? DeviceType { get; private set; }

    public string? Browser { get; private set; }

    public string? OperatingSystem { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public DateTimeOffset LastActivityAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? RevokedAt { get; private set; }

    public UserSessionStatus Status { get; private set; }
        = UserSessionStatus.Active;

    public User User { get; private set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; private set; }
        = new List<RefreshToken>();

    protected UserSession()
    {
    }

    public UserSession(
        Guid userId,
        string? deviceName = null,
        string? deviceType = null,
        string? browser = null,
        string? operatingSystem = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;

        DeviceName = Normalize(deviceName);
        DeviceType = Normalize(deviceType);
        Browser = Normalize(browser);
        OperatingSystem = Normalize(operatingSystem);
        IpAddress = Normalize(ipAddress);
        UserAgent = Normalize(userAgent);
    }

    public bool IsActive =>
        Status == UserSessionStatus.Active &&
        !RevokedAt.HasValue;

    public void UpdateDeviceInfo(
        string? deviceName,
        string? deviceType,
        string? browser,
        string? operatingSystem,
        string? ipAddress,
        string? userAgent)
    {
        EnsureActive();

        DeviceName = Normalize(deviceName);
        DeviceType = Normalize(deviceType);
        Browser = Normalize(browser);
        OperatingSystem = Normalize(operatingSystem);
        IpAddress = Normalize(ipAddress);
        UserAgent = Normalize(userAgent);

        Touch();
    }

    public void Touch()
    {
        EnsureActive();

        LastActivityAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Revoke()
    {
        if (Status == UserSessionStatus.Revoked)
            return;

        Status = UserSessionStatus.Revoked;

        RevokedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Expire()
    {
        if (Status != UserSessionStatus.Active)
            return;

        Status = UserSessionStatus.Expired;

        MarkUpdated();
    }

    private void EnsureActive()
    {
        if (!IsActive)
            throw new InvalidOperationException(
                "Session không còn active.");
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
