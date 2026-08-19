using HanYu.Domain.Entities.Lesson;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonSectionAssetConfiguration
    : AuditableEntityConfigurationBase<LessonSectionAsset>
{
    public override void Configure(EntityTypeBuilder<LessonSectionAsset> builder)
    {
        base.Configure(builder);

        builder.ToTable("lesson_section_assets");

        builder.Property(x => x.CaptionVi)
            .HasColumnType("text");

        builder.HasIndex(x => new { x.LessonSectionId, x.LessonAssetId })
            .IsUnique();

        builder.HasIndex(x => new { x.LessonSectionId, x.SortOrder });

        builder.HasOne(x => x.LessonSection)
            .WithMany()
            .HasForeignKey(x => x.LessonSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LessonAsset)
            .WithMany()
            .HasForeignKey(x => x.LessonAssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
