using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserLoginHistoryConfiguration
    : EntityConfigurationBase<UserLoginHistory>
{
    public override void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_login_histories");

        builder.Property(x => x.IpAddress)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent).HasColumnType("text");
        builder.Property(x => x.DeviceName).HasMaxLength(160);
        builder.Property(x => x.Browser).HasMaxLength(100);
        builder.Property(x => x.OperatingSystem).HasMaxLength(100);
        builder.Property(x => x.FailureReason).HasMaxLength(200);
        builder.Property(x => x.AttemptedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.AttemptedAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
