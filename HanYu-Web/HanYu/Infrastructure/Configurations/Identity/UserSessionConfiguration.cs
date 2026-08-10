using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserSessionConfiguration
    : TimestampedEntityConfigurationBase<UserSession>
{
    public override void Configure(EntityTypeBuilder<UserSession> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_sessions");

        builder.Property(x => x.SessionKey).IsRequired();
        builder.HasIndex(x => x.SessionKey).IsUnique();

        builder.Property(x => x.DeviceName).HasMaxLength(160);
        builder.Property(x => x.DeviceType).HasMaxLength(50);
        builder.Property(x => x.Browser).HasMaxLength(100);
        builder.Property(x => x.OperatingSystem).HasMaxLength(100);
        builder.Property(x => x.UserAgent).HasColumnType("text");

        builder.Property(x => x.IpAddress)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.LastActivityAt).HasColumnType("timestamptz");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.LastActivityAt });

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
