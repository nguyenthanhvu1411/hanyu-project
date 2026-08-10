using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class VocabularyMeaningConfiguration
    : AuditableEntityConfigurationBase<VocabularyMeaning>
{
    public override void Configure(EntityTypeBuilder<VocabularyMeaning> builder)
    {
        base.Configure(builder);

        builder.ToTable("vocabulary_meanings");

        builder.Property(x => x.MeaningVi)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.UsageNoteVi)
            .HasColumnType("text");

        builder.HasIndex(x => new { x.VocabularyId, x.SenseOrder })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.Vocabulary)
            .WithMany(x => x.Meanings)
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
