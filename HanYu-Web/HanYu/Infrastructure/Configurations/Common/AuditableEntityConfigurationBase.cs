using HanYu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Common;

public abstract class AuditableEntityConfigurationBase<TEntity> : TimestampedEntityConfigurationBase<TEntity>
    where TEntity : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Global query filter for soft delete
        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
