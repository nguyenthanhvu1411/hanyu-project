using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizAttemptQuestionConfiguration
    : EntityConfigurationBase<QuizAttemptQuestion>
{
    public override void Configure(EntityTypeBuilder<QuizAttemptQuestion> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_attempt_questions");

        builder.Property(x => x.QuestionSnapshotJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasIndex(x => new { x.AttemptId, x.SortOrder })
            .IsUnique();

        builder.HasOne(x => x.Attempt)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
