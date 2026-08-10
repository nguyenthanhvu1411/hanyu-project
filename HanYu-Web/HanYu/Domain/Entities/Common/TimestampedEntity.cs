namespace HanYu.Domain.Entities;

public abstract class TimestampedEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; protected set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; protected set; }
        = DateTimeOffset.UtcNow;

    protected void MarkUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
