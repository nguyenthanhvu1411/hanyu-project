using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class VocabularyExampleConfiguration
    : AuditableEntityConfigurationBase<VocabularyExample>
{
    public override void Configure(EntityTypeBuilder<VocabularyExample> builder)
    {
        base.Configure(builder);

        builder.ToTable("vocabulary_examples");

        builder.Property(x => x.SentenceZh).HasColumnType("text").IsRequired();
        builder.Property(x => x.SentencePinyin).HasColumnType("text").IsRequired();
        builder.Property(x => x.SentenceVi).HasColumnType("text").IsRequired();
        builder.Property(x => x.SourceNote).HasColumnType("text");

        builder.Property(x => x.Difficulty);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.VocabularyId, x.Status });

        builder.HasOne(x => x.Vocabulary)
            .WithMany(x => x.Examples)
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AudioAsset)
            .WithMany()
            .HasForeignKey(x => x.AudioAssetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
