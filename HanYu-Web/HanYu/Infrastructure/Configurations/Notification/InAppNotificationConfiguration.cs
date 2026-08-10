using HanYu.Domain.Entities.Notification;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Notification;

public sealed class InAppNotificationConfiguration
    : EntityConfigurationBase<InAppNotification>
{
    public override void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        base.Configure(builder);

        builder.ToTable("in_app_notifications");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Title).HasMaxLength(220).IsRequired();
        builder.Property(x => x.Message).HasColumnType("text").IsRequired();
        builder.Property(x => x.ActionUrl).HasColumnType("text");
        builder.Property(x => x.MetadataJson).HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");
        builder.Property(x => x.ReadAt).HasColumnType("timestamptz");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.ReadAt });

        // Partial index: unread notifications per user — most UI queries filter on "unread"
        builder.HasIndex(x => new { x.UserId, x.CreatedAt })
            .HasFilter("read_at IS NULL")
            .HasDatabaseName("ix_in_app_notifications_user_unread");

        // Partial index: for expiry cleanup background job
        builder.HasIndex(x => x.ExpiresAt)
            .HasFilter("expires_at IS NOT NULL")
            .HasDatabaseName("ix_in_app_notifications_expires_not_null");
    }
}
