using HanYu.Domain.Entities.Lesson;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonSectionConfiguration
    : AuditableEntityConfigurationBase<LessonSection>
{
    public override void Configure(EntityTypeBuilder<LessonSection> builder)
    {
        base.Configure(builder);

        builder.ToTable("lesson_sections");

        builder.Property(x => x.SectionType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.TitleVi)
            .HasMaxLength(220);

        builder.Property(x => x.ContentVi)
            .HasColumnType("text");

        builder.HasIndex(x => new { x.LessonId, x.SortOrder })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
