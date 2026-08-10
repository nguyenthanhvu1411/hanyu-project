using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizQuestionOptionConfiguration
    : AuditableEntityConfigurationBase<QuizQuestionOption>
{
    public override void Configure(EntityTypeBuilder<QuizQuestionOption> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_question_options");

        builder.Property(x => x.OptionText).HasColumnType("text").IsRequired();
        builder.Property(x => x.OptionPinyin).HasColumnType("text");
        builder.Property(x => x.ExplanationVi).HasColumnType("text");

        builder.HasIndex(x => new { x.QuestionId, x.SortOrder })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.Question)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
