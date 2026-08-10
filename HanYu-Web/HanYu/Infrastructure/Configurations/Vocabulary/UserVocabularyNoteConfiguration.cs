using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class UserVocabularyNoteConfiguration
    : TimestampedEntityConfigurationBase<UserVocabularyNote>
{
    public override void Configure(EntityTypeBuilder<UserVocabularyNote> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_vocabulary_notes");

        builder.Property(x => x.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.VocabularyId })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
