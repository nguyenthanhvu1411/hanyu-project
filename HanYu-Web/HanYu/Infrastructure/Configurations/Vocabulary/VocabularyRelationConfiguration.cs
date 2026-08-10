using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class VocabularyRelationConfiguration
    : AuditableEntityConfigurationBase<VocabularyRelation>
{
    public override void Configure(EntityTypeBuilder<VocabularyRelation> builder)
    {
        base.Configure(builder);

        builder.ToTable("vocabulary_relations");

        builder.Property(x => x.RelationType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.NoteVi).HasColumnType("text");

        builder.HasIndex(x => new
        {
            x.VocabularyId,
            x.RelatedVocabularyId,
            x.RelationType
        })
        .IsUnique()
        .HasFilter("deleted_at IS NULL");

        builder.ToTable("vocabulary_relations", t => t.HasCheckConstraint("ck_vocabulary_relations_not_self", "vocabulary_id <> related_vocabulary_id"));

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedVocabulary)
            .WithMany()
            .HasForeignKey(x => x.RelatedVocabularyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
