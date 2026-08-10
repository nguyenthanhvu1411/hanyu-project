using HanYu.Domain.Entities.Notification;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Notification;

public sealed class NotificationDeliveryConfiguration
    : EntityConfigurationBase<NotificationDelivery>
{
    public override void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        base.Configure(builder);

        builder.ToTable("notification_deliveries");

        builder.Property(x => x.Channel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Destination).HasMaxLength(320);
        builder.Property(x => x.Provider).HasMaxLength(50);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200);
        builder.Property(x => x.FailureCode).HasMaxLength(80);
        builder.Property(x => x.FailureReason).HasColumnType("text");

        builder.Property(x => x.LastAttemptAt).HasColumnType("timestamptz");
        builder.Property(x => x.SentAt).HasColumnType("timestamptz");
        builder.Property(x => x.DeliveredAt).HasColumnType("timestamptz");
        builder.Property(x => x.FailedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.NotificationId, x.Channel });
        builder.HasIndex(x => new { x.Status, x.LastAttemptAt });

        builder.HasOne(x => x.Notification)
            .WithMany()
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
