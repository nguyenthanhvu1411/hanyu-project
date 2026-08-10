using HanYu.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizQuestionBankItemConfiguration
    : IEntityTypeConfiguration<QuizQuestionBankItem>
{
    public void Configure(EntityTypeBuilder<QuizQuestionBankItem> builder)
    {
        builder.ToTable("quiz_question_bank_items");

        builder.HasKey(x => new { x.QuestionBankId, x.QuestionId });

        builder.Property(x => x.AddedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.QuestionBankId, x.SortOrder });

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.QuestionBankId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
