using HanYu.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Course;

public sealed class CourseConfiguration
    : IEntityTypeConfiguration<Domain.Entities.Course.Course>
{
    public void Configure(
        EntityTypeBuilder<Domain.Entities.Course.Course> builder)
    {
        builder.ToTable(
            "courses",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_courses_sort_order",
                    "sort_order >= 0");

                table.HasCheckConstraint(
                    "ck_courses_estimated_minutes",
                    "estimated_minutes IS NULL OR estimated_minutes > 0");
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
        // BASIC INFORMATION
        // =====================================================

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Slug)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.TitleVi)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ShortDescriptionVi)
            .HasMaxLength(500);

        builder.Property(x => x.DescriptionVi)
            .HasColumnType("text");

        // =====================================================
        // HSK LEVEL
        // =====================================================

        builder.Property(x => x.HskLevelId);

        builder.HasOne(x => x.HskLevel)
            .WithMany()
            .HasForeignKey(x => x.HskLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.HskLevelId);

        // =====================================================
        // DISPLAY
        // =====================================================

        builder.Property(x => x.CoverImageUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.EstimatedMinutes);

        // =====================================================
        // STATUS
        // =====================================================

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.IsFeatured)
            .HasDefaultValue(false)
            .IsRequired();

        // =====================================================
        // PUBLISH / ARCHIVE
        // =====================================================

        builder.Property(x => x.PublishedAt)
            .HasColumnType("timestamptz");

        builder.Property(x => x.PublishedById);

        builder.Property(x => x.ArchivedAt)
            .HasColumnType("timestamptz");

        builder.Property(x => x.ArchivedById);

        // =====================================================
        // CONCURRENCY
        // =====================================================

        builder.Property(x => x.ConcurrencyToken)
            .IsRequired()
            .IsConcurrencyToken();

        // =====================================================
        // COMMON / AUDIT
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

        // IsDeleted là calculated property từ DeletedAt
        // nên không tạo column riêng.
        builder.Ignore(x => x.IsDeleted);

        // =====================================================
        // RELATIONSHIPS
        // =====================================================

        builder.HasMany(x => x.Chapters)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Prerequisites)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RequiredByCourses)
            .WithOne(x => x.RequiredCourse)
            .HasForeignKey(x => x.RequiredCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // INDEXES
        // =====================================================

        builder.HasIndex(x => x.TitleVi);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.IsFeatured);

        builder.HasIndex(x => x.SortOrder);

        builder.HasIndex(x => x.PublishedAt);

        builder.HasIndex(x => x.DeletedAt);

        builder.HasIndex(
            x => new
            {
                x.HskLevelId,
                x.Status,
                x.IsActive
            });

        builder.HasIndex(
            x => new
            {
                x.Status,
                x.IsFeatured,
                x.SortOrder
            });

        // =====================================================
        // SOFT DELETE
        // =====================================================

        builder.HasQueryFilter(
            x => x.DeletedAt == null);
    }
}
