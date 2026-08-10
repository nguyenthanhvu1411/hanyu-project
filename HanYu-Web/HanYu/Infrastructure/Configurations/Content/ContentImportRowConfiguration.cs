using HanYu.Domain.Entities.Content;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Content;

public sealed class ContentImportRowConfiguration
    : EntityConfigurationBase<ContentImportRow>
{
    public override void Configure(EntityTypeBuilder<ContentImportRow> builder)
    {
        base.Configure(builder);

        builder.ToTable("content_import_rows");

        builder.Property(x => x.SourceJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.ErrorCode).HasMaxLength(80);
        builder.Property(x => x.ErrorMessage).HasColumnType("text");
        builder.Property(x => x.ProcessedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.ImportJobId, x.RowNumber })
            .IsUnique();

        builder.HasOne(x => x.ImportJob)
            .WithMany(x => x.Rows)
            .HasForeignKey(x => x.ImportJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
