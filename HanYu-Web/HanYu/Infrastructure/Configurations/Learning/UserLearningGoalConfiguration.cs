using HanYu.Domain.Entities.Learning;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Learning;

public sealed class UserLearningGoalConfiguration
    : TimestampedEntityConfigurationBase<UserLearningGoal>
{
    public override void Configure(EntityTypeBuilder<UserLearningGoal> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_learning_goals");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.PausedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.ToTable("user_learning_goals", t => t.HasCheckConstraint("ck_user_learning_goals_hsk", "target_hsk_level >= 1 AND target_hsk_level <= 6"));

        builder.ToTable("user_learning_goals", t => t.HasCheckConstraint("ck_user_learning_goals_daily_minutes", "daily_goal_minutes >= 1"));

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
