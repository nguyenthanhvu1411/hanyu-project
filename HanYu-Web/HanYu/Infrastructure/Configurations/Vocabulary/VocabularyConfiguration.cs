using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class VocabularyConfiguration
    : AuditableEntityConfigurationBase<HanYu.Domain.Entities.Vocabulary.Vocabulary>
{
    public override void Configure(EntityTypeBuilder<HanYu.Domain.Entities.Vocabulary.Vocabulary> builder)
    {
        base.Configure(builder);

        builder.ToTable("vocabularies");

        builder.Property(x => x.Simplified)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Traditional)
            .HasMaxLength(100);

        builder.Property(x => x.Pinyin)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PinyinNormalized)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PrimaryMeaningVi)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.NotesVi)
            .HasColumnType("text");

        builder.Property(x => x.Difficulty);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Version);

        builder.Property(x => x.PublishedAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.HskLevelId, x.Status });
        builder.HasIndex(x => x.Simplified);
        builder.HasIndex(x => x.Traditional);
        builder.HasIndex(x => x.PinyinNormalized);

        builder.HasIndex(x => new
        {
            x.Simplified,
            x.PinyinNormalized,
            x.HskLevelId
        })
        .IsUnique()
        .HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.HskLevel)
            .WithMany()
            .HasForeignKey(x => x.HskLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PartOfSpeech)
            .WithMany()
            .HasForeignKey(x => x.PartOfSpeechId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Topic)
            .WithMany()
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
