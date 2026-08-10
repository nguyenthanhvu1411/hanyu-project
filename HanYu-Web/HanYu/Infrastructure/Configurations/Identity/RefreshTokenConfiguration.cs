using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class RefreshTokenConfiguration
    : EntityConfigurationBase<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("refresh_tokens");

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.FamilyId);
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => new { x.UserId, x.RevokedAt });

        builder.Property(x => x.IssuedAt).HasColumnType("timestamptz");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");
        builder.Property(x => x.UsedAt).HasColumnType("timestamptz");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamptz");

        builder.Property(x => x.CreatedByIp)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.RevokedByIp)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent).HasColumnType("text");
        builder.Property(x => x.RevokeReason).HasMaxLength(200);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UserSession)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserSessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
