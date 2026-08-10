using HanYu.Domain.Entities.Gamification;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Gamification;

public sealed class AchievementConfiguration
    : AuditableEntityConfigurationBase<Achievement>
{
    public override void Configure(EntityTypeBuilder<Achievement> builder)
    {
        base.Configure(builder);

        builder.ToTable("achievements");

        builder.Property(x => x.Code).HasMaxLength(60).IsRequired();
        builder.Property(x => x.NameVi).HasMaxLength(160).IsRequired();
        builder.Property(x => x.DescriptionVi).HasColumnType("text");
        builder.Property(x => x.IconUrl).HasColumnType("text");

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.SortOrder });
    }
}
