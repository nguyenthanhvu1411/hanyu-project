using HanYu.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizQuestionTagConfiguration
    : IEntityTypeConfiguration<QuizQuestionTag>
{
    public void Configure(EntityTypeBuilder<QuizQuestionTag> builder)
    {
        builder.ToTable("quiz_question_tags");

        builder.HasKey(x => new { x.QuestionId, x.TagId });

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.QuestionTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
