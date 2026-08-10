using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizTagConfiguration : AuditableEntityConfigurationBase<QuizTag>
{
    public override void Configure(EntityTypeBuilder<QuizTag> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_tags");

        builder.Property(x => x.Slug).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameVi).HasMaxLength(100);
        builder.Property(x => x.DescriptionVi).HasColumnType("text");

        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
