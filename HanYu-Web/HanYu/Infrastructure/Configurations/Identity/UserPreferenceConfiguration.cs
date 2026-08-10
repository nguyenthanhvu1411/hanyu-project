using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserPreferenceConfiguration
    : TimestampedEntityConfigurationBase<UserPreference>
{
    public override void Configure(
        EntityTypeBuilder<UserPreference> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_preferences");

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.Theme)
            .HasMaxLength(20);

        builder.Property(x => x.DefaultFlashcardMode)
            .HasMaxLength(30);

        builder.Property(x => x.AudioPlaybackRate)
            .HasPrecision(3, 2);

        builder.ToTable("user_preferences", t => t.HasCheckConstraint("ck_user_preferences_audio_playback_rate", "audio_playback_rate >= 0.50 AND audio_playback_rate <= 2.00"));

        builder.HasOne(x => x.User)
            .WithOne(x => x.Preference)
            .HasForeignKey<UserPreference>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
