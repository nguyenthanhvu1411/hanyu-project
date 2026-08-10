using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserConsentConfiguration
    : EntityConfigurationBase<UserConsent>
{
    public override void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_consents");

        builder.Property(x => x.ConsentType)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Version)
            .HasMaxLength(30);

        builder.Property(x => x.GrantedAt).HasColumnType("timestamptz");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.ConsentType, x.Version });

        builder.HasOne(x => x.User)
            .WithMany(x => x.Consents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
