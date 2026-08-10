namespace HanYu.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; protected set; }

    public Guid PublicId { get; protected set; }
        = Guid.NewGuid();
}
