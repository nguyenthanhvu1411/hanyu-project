using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Operations;

public sealed class ProductEventConfiguration
    : EntityConfigurationBase<ProductEvent>
{
    public override void Configure(EntityTypeBuilder<ProductEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("product_events");

        builder.Property(x => x.EventName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(80);
        builder.Property(x => x.EntityPublicId).HasMaxLength(100);
        builder.Property(x => x.PropertiesJson).HasColumnType("jsonb");
        builder.Property(x => x.PagePath).HasMaxLength(500);
        builder.Property(x => x.Referrer).HasColumnType("text");
        builder.Property(x => x.DeviceType).HasMaxLength(50);
        builder.Property(x => x.OccurredAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.EventName, x.OccurredAt });
        builder.HasIndex(x => new { x.UserId, x.OccurredAt });
    }
}
