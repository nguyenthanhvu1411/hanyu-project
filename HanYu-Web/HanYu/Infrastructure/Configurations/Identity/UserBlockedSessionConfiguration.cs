using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserBlockedSessionConfiguration
    : EntityConfigurationBase<UserBlockedSession>
{
    public override void Configure(EntityTypeBuilder<UserBlockedSession> builder)
    {
        base.Configure(builder);
        builder.ToTable("user_blocked_sessions");

        builder.Property(x => x.Reason).HasMaxLength(300).IsRequired();
        builder.Property(x => x.BlockedAt).HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");

        builder.Property(x => x.IpAddress)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.HasIndex(x => x.UserSessionId);
        builder.HasIndex(x => new { x.UserId, x.BlockedAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UserSession)
            .WithMany()
            .HasForeignKey(x => x.UserSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
