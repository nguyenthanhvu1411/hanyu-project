using HanYu.Domain.Entities.Lesson;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonConfiguration : AuditableEntityConfigurationBase<HanYu.Domain.Entities.Lesson.Lesson>
{
    public override void Configure(EntityTypeBuilder<HanYu.Domain.Entities.Lesson.Lesson> builder)
    {
        base.Configure(builder);

        builder.ToTable("lessons");

        builder.Property(x => x.Slug)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.TitleVi)
            .HasMaxLength(220)
            .IsRequired();

        builder.Property(x => x.ShortDescriptionVi)
            .HasMaxLength(320);

        builder.Property(x => x.DescriptionVi).HasColumnType("text");
        builder.Property(x => x.ObjectiveVi).HasColumnType("text");
        builder.Property(x => x.CoverImageUrl).HasColumnType("text");

        builder.Property(x => x.EstimatedMinutes);

        builder.Property(x => x.Difficulty);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Version);

        builder.Property(x => x.PublishedAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.HskLevelId, x.Status, x.SortOrder });
        builder.HasIndex(x => new { x.TopicId, x.Status });

        builder.ToTable("lessons", t => t.HasCheckConstraint("ck_lessons_estimated_minutes", "estimated_minutes >= 1 AND estimated_minutes <= 120"));

        builder.ToTable("lessons", t => t.HasCheckConstraint("ck_lessons_difficulty", "difficulty >= 1 AND difficulty <= 5"));

        builder.HasOne(x => x.HskLevel)
            .WithMany()
            .HasForeignKey(x => x.HskLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Topic)
            .WithMany()
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.CourseChapter)
            .WithMany(x => x.Lessons)
            .HasForeignKey(x => x.CourseChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CourseChapterId);

        builder.HasIndex(
            x => new
            {
                x.CourseChapterId,
                x.Status,
                x.SortOrder
            });

        builder.HasIndex(
                x => new
                {
                    x.CourseChapterId,
                    x.SortOrder
                })
            .IsUnique()
            .HasFilter(
                "course_chapter_id IS NOT NULL AND deleted_at IS NULL");
    }
}
