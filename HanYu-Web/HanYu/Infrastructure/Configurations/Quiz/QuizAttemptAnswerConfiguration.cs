using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizAttemptAnswerConfiguration
    : TimestampedEntityConfigurationBase<QuizAttemptAnswer>
{
    public override void Configure(EntityTypeBuilder<QuizAttemptAnswer> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_attempt_answers");

        builder.Property(x => x.AnswerText).HasColumnType("text");
        builder.Property(x => x.AnswerJson).HasColumnType("jsonb");

        builder.Property(x => x.EarnedPoints).HasPrecision(8, 2);
        builder.Property(x => x.AnsweredAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.AttemptId, x.QuestionId })
            .IsUnique();

        builder.HasOne(x => x.Attempt)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedOption)
            .WithMany()
            .HasForeignKey(x => x.SelectedOptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
