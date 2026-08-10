using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class HskLevelConfiguration : AuditableEntityConfigurationBase<HskLevel>
{
    public override void Configure(EntityTypeBuilder<HskLevel> builder)
    {
        base.Configure(builder);

        builder.ToTable("hsk_levels");



        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.NameVi)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive);

        builder.HasIndex(x => x.SortOrder);
    }
}
