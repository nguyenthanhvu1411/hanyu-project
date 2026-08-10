using HanYu.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Learning;

public sealed class UserLessonBookmarkConfiguration
    : IEntityTypeConfiguration<UserLessonBookmark>
{
    public void Configure(EntityTypeBuilder<UserLessonBookmark> builder)
    {
        builder.ToTable("user_lesson_bookmarks");

        builder.HasKey(x => new { x.UserId, x.LessonId });

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Lesson)
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
