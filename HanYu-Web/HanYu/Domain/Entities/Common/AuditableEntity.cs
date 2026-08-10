namespace HanYu.Domain.Entities;

public abstract class AuditableEntity : TimestampedEntity
{
    public Guid? CreatedById { get; protected set; }

    public Guid? UpdatedById { get; protected set; }

    public DateTimeOffset? DeletedAt { get; protected set; }

    public Guid? DeletedById { get; protected set; }

    public bool IsDeleted => DeletedAt.HasValue;

    public void SetCreatedBy(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        CreatedById = userId;
        UpdatedById = userId;
    }

    public void MarkAsUpdated(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        UpdatedById = userId;
        MarkUpdated();
    }

    public void SoftDelete(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        if (IsDeleted)
            return;

        DeletedAt = DateTimeOffset.UtcNow;
        DeletedById = userId;
        UpdatedById = userId;

        MarkUpdated();
    }

    public void Restore(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        if (!IsDeleted)
            return;

        DeletedAt = null;
        DeletedById = null;
        UpdatedById = userId;

        MarkUpdated();
    }
}
