using HanYu.Domain.Entities.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Analytics;

public sealed class DailyLearningStatConfiguration
    : IEntityTypeConfiguration<DailyLearningStat>
{
    public void Configure(EntityTypeBuilder<DailyLearningStat> builder)
    {
        builder.ToTable("daily_learning_stats");

        builder.HasKey(x => new { x.UserId, x.StatDate });

        builder.Property(x => x.StatDate)
            .HasColumnType("date");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.StatDate });
    }
}
