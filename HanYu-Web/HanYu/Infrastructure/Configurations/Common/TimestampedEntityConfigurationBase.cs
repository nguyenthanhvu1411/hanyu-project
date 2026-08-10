using HanYu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Common;

public abstract class TimestampedEntityConfigurationBase<TEntity> : EntityConfigurationBase<TEntity>
    where TEntity : TimestampedEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
    }
}
