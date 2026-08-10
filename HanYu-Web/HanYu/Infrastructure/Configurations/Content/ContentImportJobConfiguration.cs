using HanYu.Domain.Entities.Content;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Content;

public sealed class ContentImportJobConfiguration
    : AuditableEntityConfigurationBase<ContentImportJob>
{
    public override void Configure(EntityTypeBuilder<ContentImportJob> builder)
    {
        base.Configure(builder);

        builder.ToTable("content_import_jobs");

        builder.Property(x => x.ImportType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StoragePath)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.ErrorMessage).HasColumnType("text");

        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}
