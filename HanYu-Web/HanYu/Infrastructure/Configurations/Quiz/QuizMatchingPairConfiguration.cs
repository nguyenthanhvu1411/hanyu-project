using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizMatchingPairConfiguration
    : AuditableEntityConfigurationBase<QuizMatchingPair>
{
    public override void Configure(EntityTypeBuilder<QuizMatchingPair> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_matching_pairs");

        builder.Property(x => x.LeftText).HasColumnType("text").IsRequired();
        builder.Property(x => x.RightText).HasColumnType("text").IsRequired();
        builder.Property(x => x.LeftPinyin).HasColumnType("text");
        builder.Property(x => x.RightPinyin).HasColumnType("text");

        builder.HasIndex(x => new { x.QuestionId, x.SortOrder })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.Question)
            .WithMany(x => x.MatchingPairs)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
