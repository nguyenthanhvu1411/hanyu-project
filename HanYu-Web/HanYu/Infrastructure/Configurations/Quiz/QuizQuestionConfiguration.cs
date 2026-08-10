using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizQuestionConfiguration
    : AuditableEntityConfigurationBase<QuizQuestion>
{
    public override void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_questions");

        builder.Property(x => x.QuestionType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Prompt).HasColumnType("text").IsRequired();
        builder.Property(x => x.PromptPinyin).HasColumnType("text");
        builder.Property(x => x.CorrectAnswerText).HasColumnType("text");
        builder.Property(x => x.ExplanationVi).HasColumnType("text");
        builder.Property(x => x.HintVi).HasColumnType("text");

        builder.Property(x => x.Points)
            .HasPrecision(8, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.QuizId, x.SortOrder })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.ToTable("quiz_questions", t => t.HasCheckConstraint("ck_quiz_questions_points", "points > 0"));

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
