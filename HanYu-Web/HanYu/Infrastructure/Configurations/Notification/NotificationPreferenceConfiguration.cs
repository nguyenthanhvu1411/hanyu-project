using HanYu.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Notification;

public sealed class NotificationPreferenceConfiguration
    : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Timezone)
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamptz");
    }
}
