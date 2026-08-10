using HanYu.Domain.Entities.Lesson;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonAssetConfiguration
    : AuditableEntityConfigurationBase<LessonAsset>
{
    public override void Configure(EntityTypeBuilder<LessonAsset> builder)
    {
        base.Configure(builder);

        builder.ToTable("lesson_assets");

        builder.Property(x => x.AssetType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Url).HasColumnType("text");
        builder.Property(x => x.CaptionVi).HasColumnType("text");

        builder.HasIndex(x => new { x.LessonId, x.SortOrder });

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AudioAsset)
            .WithMany()
            .HasForeignKey(x => x.AudioAssetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
