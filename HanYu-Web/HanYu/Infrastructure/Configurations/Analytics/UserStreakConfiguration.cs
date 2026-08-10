using HanYu.Domain.Entities.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Analytics;

public sealed class UserStreakConfiguration
    : IEntityTypeConfiguration<UserStreak>
{
    public void Configure(EntityTypeBuilder<UserStreak> builder)
    {
        builder.ToTable("user_streaks");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.LastLearningDate).HasColumnType("date");
        builder.Property(x => x.CurrentStreakStartedAt).HasColumnType("date");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");
    }
}
