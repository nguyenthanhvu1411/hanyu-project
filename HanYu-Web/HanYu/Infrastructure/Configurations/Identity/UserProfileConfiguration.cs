using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserProfileConfiguration
    : TimestampedEntityConfigurationBase<UserProfile>
{
    public override void Configure(
        EntityTypeBuilder<UserProfile> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_profiles");

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.AvatarUrl)
            .HasColumnType("text");

        builder.Property(x => x.CurrentHskLevel);

        builder.Property(x => x.DailyGoalMinutes);

        builder.Property(x => x.Timezone)
            .HasMaxLength(100);

        builder.Property(x => x.UiLanguage)
            .HasMaxLength(10);

        builder.Property(x => x.OnboardingCompleted);

        builder.Property(x => x.OnboardingCompletedAt)
            .HasColumnType("timestamptz");

        builder.ToTable("user_profiles", t => t.HasCheckConstraint("ck_user_profiles_current_hsk_level", "current_hsk_level >= 1 AND current_hsk_level <= 6"));

        builder.ToTable("user_profiles", t => t.HasCheckConstraint("ck_user_profiles_daily_goal_minutes", "daily_goal_minutes >= 5 AND daily_goal_minutes <= 180"));

        builder.HasOne(x => x.User)
            .WithOne(x => x.Profile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
