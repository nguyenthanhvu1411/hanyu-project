using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Vocabulary;

public sealed class AudioAssetConfiguration
    : AuditableEntityConfigurationBase<AudioAsset>
{
    public override void Configure(EntityTypeBuilder<AudioAsset> builder)
    {
        base.Configure(builder);
        builder.ToTable("audio_assets");

        builder.Property(x => x.StoragePath).HasColumnType("text").IsRequired();
        builder.HasIndex(x => x.StoragePath).IsUnique();

        builder.Property(x => x.PublicUrl).HasColumnType("text");
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Voice).HasMaxLength(80);
        builder.Property(x => x.Provider).HasMaxLength(80);
        builder.Property(x => x.LanguageCode).HasMaxLength(20);
        builder.Property(x => x.Checksum).HasMaxLength(128);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        builder.ToTable("audio_assets", t => t.HasCheckConstraint("ck_audio_assets_duration_ms", "duration_ms IS NULL OR duration_ms >= 0"));
        builder.ToTable("audio_assets", t => t.HasCheckConstraint("ck_audio_assets_file_size", "file_size_bytes IS NULL OR file_size_bytes >= 0"));
    }
}
