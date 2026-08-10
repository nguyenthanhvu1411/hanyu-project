using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserDataExportJobConfiguration
    : TimestampedEntityConfigurationBase<UserDataExportJob>
{
    public override void Configure(EntityTypeBuilder<UserDataExportJob> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_data_export_jobs");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.StoragePath).HasColumnType("text");
        builder.Property(x => x.ErrorMessage).HasColumnType("text");

        builder.Property(x => x.RequestedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
