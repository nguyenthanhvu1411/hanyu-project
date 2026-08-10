using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonVocabularyConfiguration
    : IEntityTypeConfiguration<LessonVocabulary>
{
    public void Configure(EntityTypeBuilder<LessonVocabulary> builder)
    {
        builder.ToTable("lesson_vocabularies");

        builder.HasKey(x => new { x.LessonId, x.VocabularyId });

        builder.HasIndex(x => new { x.LessonId, x.SortOrder })
            .IsUnique();

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.LessonVocabularies)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
