using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizQuestionBankConfiguration
    : AuditableEntityConfigurationBase<QuizQuestionBank>
{
    public override void Configure(EntityTypeBuilder<QuizQuestionBank> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_question_banks");

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NameVi).HasMaxLength(160).IsRequired();
        builder.Property(x => x.DescriptionVi).HasColumnType("text");

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => new { x.HskLevelId, x.IsActive });
    }
}
