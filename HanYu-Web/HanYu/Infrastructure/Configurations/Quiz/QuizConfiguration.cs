using QuizEntity = HanYu.Domain.Entities.Quiz.Quiz;
using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizConfiguration : AuditableEntityConfigurationBase<QuizEntity>
{
    public override void Configure(EntityTypeBuilder<QuizEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("quizzes");

        builder.Property(x => x.Version);

        builder.Property(x => x.TitleVi)
            .HasMaxLength(220)
            .IsRequired();

        builder.Property(x => x.DescriptionVi)
            .HasColumnType("text");

        builder.Property(x => x.QuizType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.PassingScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.ShuffleMode)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.FeedbackMode)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.PublishedAt)
            .HasColumnType("timestamptz");

        builder.ToTable("quizzes", t => t.HasCheckConstraint("ck_quizzes_passing_score", "passing_score >= 0 AND passing_score <= 100"));

        builder.ToTable("quizzes", t => t.HasCheckConstraint("ck_quizzes_time_limit", "time_limit_seconds IS NULL OR time_limit_seconds > 0"));

        builder.HasIndex(x => new { x.LessonId, x.Status });

        builder.HasOne(x => x.Lesson)
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
