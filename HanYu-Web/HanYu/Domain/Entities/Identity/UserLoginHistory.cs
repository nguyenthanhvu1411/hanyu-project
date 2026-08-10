using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Identity;

public class UserLoginHistory : BaseEntity
{
    public Guid UserId { get; private set; }

    public bool IsSuccessful { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? DeviceName { get; private set; }

    public string? Browser { get; private set; }

    public string? OperatingSystem { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTimeOffset AttemptedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public User User { get; private set; } = null!;

    protected UserLoginHistory()
    {
    }

    public UserLoginHistory(
        Guid userId,
        bool isSuccessful,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceName = null,
        string? browser = null,
        string? operatingSystem = null,
        string? failureReason = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (isSuccessful &&
            !string.IsNullOrWhiteSpace(failureReason))
        {
            throw new ArgumentException(
                "Login thành công không được có FailureReason.",
                nameof(failureReason));
        }

        UserId = userId;
        IsSuccessful = isSuccessful;

        IpAddress = Normalize(ipAddress);
        UserAgent = Normalize(userAgent);
        DeviceName = Normalize(deviceName);
        Browser = Normalize(browser);
        OperatingSystem =
            Normalize(operatingSystem);

        FailureReason =
            isSuccessful
                ? null
                : Normalize(failureReason);
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
