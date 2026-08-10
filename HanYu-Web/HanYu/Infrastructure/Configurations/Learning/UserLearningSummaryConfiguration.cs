using HanYu.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Learning;

public sealed class UserLearningSummaryConfiguration
    : IEntityTypeConfiguration<UserLearningSummary>
{
    public void Configure(EntityTypeBuilder<UserLearningSummary> builder)
    {
        builder.ToTable("user_learning_summaries");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.OverallMasteryPercent)
            .HasPrecision(5, 2);

        builder.Property(x => x.LastLearningAt).HasColumnType("timestamptz");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");

        builder.ToTable("user_learning_summaries", t => t.HasCheckConstraint("ck_user_learning_summaries_mastery", "overall_mastery_percent >= 0 AND overall_mastery_percent <= 100"));
    }
}
