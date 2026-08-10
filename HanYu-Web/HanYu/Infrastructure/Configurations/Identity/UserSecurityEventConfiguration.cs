using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserSecurityEventConfiguration
    : EntityConfigurationBase<UserSecurityEvent>
{
    public override void Configure(EntityTypeBuilder<UserSecurityEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_security_events");

        builder.Property(x => x.EventType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.IpAddress)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent).HasColumnType("text");
        builder.Property(x => x.MetadataJson).HasColumnType("jsonb");
        builder.Property(x => x.OccurredAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.OccurredAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
