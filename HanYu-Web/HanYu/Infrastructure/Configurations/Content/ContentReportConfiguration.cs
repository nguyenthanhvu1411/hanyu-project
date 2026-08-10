using HanYu.Domain.Entities.Content;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Content;

public sealed class ContentReportConfiguration
    : TimestampedEntityConfigurationBase<ContentReport>
{
    public override void Configure(EntityTypeBuilder<ContentReport> builder)
    {
        base.Configure(builder);

        builder.ToTable("content_reports");

        builder.Property(x => x.EntityType)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Reason)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Description).HasColumnType("text");
        builder.Property(x => x.ResolutionNote).HasColumnType("text");
        builder.Property(x => x.ResolvedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.Status, x.EntityType });
        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}
