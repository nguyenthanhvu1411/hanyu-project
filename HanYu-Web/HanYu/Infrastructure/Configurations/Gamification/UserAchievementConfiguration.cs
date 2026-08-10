using HanYu.Domain.Entities.Gamification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Gamification;

public sealed class UserAchievementConfiguration
    : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("user_achievements");

        builder.HasKey(x => new { x.UserId, x.AchievementId });

        builder.Property(x => x.UnlockedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.UnlockedAt });
    }
}
