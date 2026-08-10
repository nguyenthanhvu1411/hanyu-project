using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Identity;

public class UserSecurityEvent : BaseEntity
{
    public Guid UserId { get; private set; }

    public UserSecurityEventType EventType { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public User User { get; private set; } = null!;

    protected UserSecurityEvent()
    {
    }

    public UserSecurityEvent(
        Guid userId,
        UserSecurityEventType eventType,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadataJson = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;
        EventType = eventType;

        IpAddress =
            Normalize(ipAddress);

        UserAgent =
            Normalize(userAgent);

        MetadataJson =
            string.IsNullOrWhiteSpace(metadataJson)
                ? null
                : metadataJson;
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
