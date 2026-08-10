using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Lesson;

public sealed class LessonPrerequisiteConfiguration
    : IEntityTypeConfiguration<LessonPrerequisite>
{
    public void Configure(EntityTypeBuilder<LessonPrerequisite> builder)
    {
        builder.ToTable("lesson_prerequisites");

        builder.HasKey(x => new { x.LessonId, x.RequiredLessonId });

        builder.ToTable("lesson_prerequisites", t => t.HasCheckConstraint("ck_lesson_prerequisites_not_self", "lesson_id <> required_lesson_id"));

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.Prerequisites)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RequiredLesson)
            .WithMany()
            .HasForeignKey(x => x.RequiredLessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
