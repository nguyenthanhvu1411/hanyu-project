using HanYu.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Course;

public sealed class CourseChapterConfiguration
    : IEntityTypeConfiguration<CourseChapter>
{
    public void Configure(
        EntityTypeBuilder<CourseChapter> builder)
    {
        builder.ToTable(
            "course_chapters",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_course_chapters_sort_order",
                    "sort_order >= 0");
            });

        // =====================================================
        // KEY
        // =====================================================

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        // =====================================================
        // COURSE
        // =====================================================

        builder.Property(x => x.CourseId)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Chapters)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // BASIC INFORMATION
        // =====================================================

        builder.Property(x => x.TitleVi)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DescriptionVi)
            .HasColumnType("text");

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        // =====================================================
        // AUDIT
        // =====================================================

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.CreatedById);

        builder.Property(x => x.UpdatedById);

        builder.Property(x => x.DeletedAt)
            .HasColumnType("timestamptz");

        builder.Property(x => x.DeletedById);

        builder.Ignore(x => x.IsDeleted);

        // =====================================================
        // CONCURRENCY
        // =====================================================

        builder.Property(x => x.ConcurrencyToken)
            .IsRequired()
            .IsConcurrencyToken();

        // =====================================================
        // INDEXES
        // =====================================================

        builder.HasIndex(x => x.CourseId);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.DeletedAt);

        // Một khóa học không nên có 2 chapter active cùng vị trí.
        builder.HasIndex(
                x => new
                {
                    x.CourseId,
                    x.SortOrder
                })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        // =====================================================
        // SOFT DELETE
        // =====================================================

        builder.HasQueryFilter(
            x => x.DeletedAt == null);
    }
}
