using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Operations;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }

    public string Action { get; private set; }
        = string.Empty;

    public string EntityType { get; private set; }
        = string.Empty;

    public string? EntityId { get; private set; }

    public string? EntityPublicId { get; private set; }

    public string? OldValuesJson { get; private set; }

    public string? NewValuesJson { get; private set; }

    public string? ChangedPropertiesJson { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? CorrelationId { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected AuditLog()
    {
    }

    public AuditLog(
        Guid? userId,
        string action,
        string entityType,
        string? entityId = null,
        string? entityPublicId = null,
        string? oldValuesJson = null,
        string? newValuesJson = null,
        string? changedPropertiesJson = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null)
    {
        if (userId.HasValue &&
            userId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException(
                "Action không được để trống.",
                nameof(action));

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException(
                "EntityType không được để trống.",
                nameof(entityType));

        UserId = userId;
        Action = action.Trim();
        EntityType = entityType.Trim();

        EntityId = Normalize(entityId);
        EntityPublicId = Normalize(entityPublicId);

        OldValuesJson = NormalizeJson(oldValuesJson);
        NewValuesJson = NormalizeJson(newValuesJson);
        ChangedPropertiesJson =
            NormalizeJson(changedPropertiesJson);

        IpAddress = Normalize(ipAddress);
        UserAgent = Normalize(userAgent);
        CorrelationId = Normalize(correlationId);
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
