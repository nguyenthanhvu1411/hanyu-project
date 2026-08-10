using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Operations;

public class ProductEvent : BaseEntity
{
    public Guid? UserId { get; private set; }

    public Guid? SessionId { get; private set; }

    public string EventName { get; private set; }
        = string.Empty;

    public string? EntityType { get; private set; }

    public string? EntityPublicId { get; private set; }

    public string? PropertiesJson { get; private set; }

    public string? PagePath { get; private set; }

    public string? Referrer { get; private set; }

    public string? DeviceType { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected ProductEvent()
    {
    }

    public ProductEvent(
        string eventName,
        Guid? userId = null,
        Guid? sessionId = null,
        string? entityType = null,
        string? entityPublicId = null,
        string? propertiesJson = null)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException(
                "EventName không được để trống.",
                nameof(eventName));

        if (userId.HasValue &&
            userId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));
        }

        if (sessionId.HasValue &&
            sessionId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "SessionId không hợp lệ.",
                nameof(sessionId));
        }

        EventName = eventName.Trim();

        UserId = userId;
        SessionId = sessionId;

        EntityType = Normalize(entityType);
        EntityPublicId = Normalize(entityPublicId);

        PropertiesJson =
            NormalizeJson(propertiesJson);
    }

    public void AttachPageContext(
        string? pagePath,
        string? referrer,
        string? deviceType)
    {
        PagePath = Normalize(pagePath);
        Referrer = Normalize(referrer);
        DeviceType = Normalize(deviceType);
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
