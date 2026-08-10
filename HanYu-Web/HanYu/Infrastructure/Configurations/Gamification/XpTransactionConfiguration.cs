using HanYu.Domain.Entities.Gamification;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Gamification;

public sealed class XpTransactionConfiguration
    : EntityConfigurationBase<XpTransaction>
{
    public override void Configure(EntityTypeBuilder<XpTransaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("xp_transactions");

        builder.Property(x => x.Reason).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SourceType).HasMaxLength(50);
        builder.Property(x => x.SourceId).HasMaxLength(100);
        builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}
