using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class TopicConfiguration : AuditableEntityConfigurationBase<Topic>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        base.Configure(builder);

        builder.ToTable("topics");

        builder.Property(x => x.Slug)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.NameVi)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(x => x.DescriptionVi)
            .HasColumnType("text");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.Status, x.SortOrder });
    }
}
