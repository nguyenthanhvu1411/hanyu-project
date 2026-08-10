using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class PartOfSpeechConfiguration
    : AuditableEntityConfigurationBase<PartOfSpeech>
{
    public override void Configure(EntityTypeBuilder<PartOfSpeech> builder)
    {
        base.Configure(builder);

        builder.ToTable("parts_of_speech");

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.NameVi)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.NameEn)
            .HasMaxLength(100);
    }
}
