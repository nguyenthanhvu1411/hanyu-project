using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class UserLessonSectionProgressConfiguration
    : IEntityTypeConfiguration<UserLessonSectionProgress>
{
    public void Configure(EntityTypeBuilder<UserLessonSectionProgress> builder)
    {
        builder.ToTable("user_lesson_section_progress");

        builder.HasKey(x => new { x.UserId, x.LessonSectionId });

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");

        builder.ToTable("user_lesson_section_progress", t => t.HasCheckConstraint("ck_user_lesson_section_progress_time", "time_spent_seconds >= 0"));

        builder.HasOne(x => x.LessonSection)
            .WithMany()
            .HasForeignKey(x => x.LessonSectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
