using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class UserLessonProgressConfiguration
    : IEntityTypeConfiguration<UserLessonProgress>
{
    public void Configure(EntityTypeBuilder<UserLessonProgress> builder)
    {
        builder.ToTable("user_lesson_progress");

        builder.HasKey(x => new { x.UserId, x.LessonId });

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.LastAccessedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");

        builder.Property(x => x.CompletionPercent)
            .HasPrecision(5, 2);

        builder.ToTable("user_lesson_progress", t => t.HasCheckConstraint("ck_user_lesson_progress_completion", "completion_percent >= 0 AND completion_percent <= 100"));

        builder.HasIndex(x => new { x.UserId, x.Status });

        // Compound index: "show in-progress lessons sorted by last accessed" — common dashboard query
        builder.HasIndex(x => new { x.UserId, x.Status, x.LastAccessedAt })
            .HasDatabaseName("ix_user_lesson_progress_user_status_last_accessed");

        // Partial index: only rows WHERE status = 'Completed' — used by analytics completion rate queries
        builder.HasIndex(x => new { x.UserId, x.CompletedAt })
            .HasFilter("completed_at IS NOT NULL")
            .HasDatabaseName("ix_user_lesson_progress_user_completed");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Lesson)
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
