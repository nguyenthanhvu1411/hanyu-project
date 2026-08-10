using HanYu.Domain.Entities.Learning;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Learning;

public sealed class LearningActivityConfiguration
    : EntityConfigurationBase<LearningActivity>
{
    public override void Configure(EntityTypeBuilder<LearningActivity> builder)
    {
        base.Configure(builder);

        builder.ToTable("learning_activities");

        builder.Property(x => x.ActivityType)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.MetadataJson).HasColumnType("jsonb");

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.StartedAt });
        builder.HasIndex(x => new { x.UserId, x.ActivityType });

        // Compound index for time-window activity queries (e.g. "activities of type X in date range")
        builder.HasIndex(x => new { x.UserId, x.ActivityType, x.StartedAt })
            .HasDatabaseName("ix_learning_activities_user_type_started");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Lesson)
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.QuizAttempt)
            .WithMany()
            .HasForeignKey(x => x.QuizAttemptId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.FlashcardSession)
            .WithMany()
            .HasForeignKey(x => x.FlashcardSessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
