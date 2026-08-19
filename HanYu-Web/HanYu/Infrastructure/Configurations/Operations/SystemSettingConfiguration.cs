using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Operations;

public sealed class SystemSettingConfiguration : TimestampedEntityConfigurationBase<SystemSetting>
{
    public override void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        base.Configure(builder);

        builder.ToTable("system_settings");
        builder.Property(x => x.Key).HasMaxLength(120).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Group).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(8000).IsRequired();
        builder.Property(x => x.ValueType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.Group);
    }
}
