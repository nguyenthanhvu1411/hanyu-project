using HanYu.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Course;

public sealed class CoursePrerequisiteConfiguration
    : IEntityTypeConfiguration<CoursePrerequisite>
{
    public void Configure(
        EntityTypeBuilder<CoursePrerequisite> builder)
    {
        builder.ToTable(
            "course_prerequisites",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_course_prerequisites_not_self",
                    "course_id <> required_course_id");

                table.HasCheckConstraint(
                    "ck_course_prerequisites_sort_order",
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
            .WithMany(x => x.Prerequisites)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // REQUIRED COURSE
        // =====================================================

        builder.Property(x => x.RequiredCourseId)
            .IsRequired();

        builder.HasOne(x => x.RequiredCourse)
            .WithMany(x => x.RequiredByCourses)
            .HasForeignKey(x => x.RequiredCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // OPTIONS
        // =====================================================

        builder.Property(x => x.IsRequired)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0)
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

        builder.HasIndex(x => x.RequiredCourseId);

        builder.HasIndex(x => x.DeletedAt);

        // Course A chỉ được khai báo Course B
        // làm prerequisite một lần.
        builder.HasIndex(
                x => new
                {
                    x.CourseId,
                    x.RequiredCourseId
                })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");

        builder.HasIndex(
            x => new
            {
                x.CourseId,
                x.SortOrder
            });

        // =====================================================
        // SOFT DELETE
        // =====================================================

        builder.HasQueryFilter(
            x => x.DeletedAt == null);
    }
}
