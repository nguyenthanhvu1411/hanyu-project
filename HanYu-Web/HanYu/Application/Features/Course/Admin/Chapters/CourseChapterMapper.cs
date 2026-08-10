using HanYu.Domain.Entities.Course;

namespace HanYu.Application.Features.Course.Admin.Chapters;

public static class CourseChapterMapper
{
    public static CourseChapterAdminDto ToDto(CourseChapter chapter)
    {
        ArgumentNullException.ThrowIfNull(chapter);

        return new CourseChapterAdminDto(
            Id: chapter.Id,
            PublicId: chapter.PublicId,
            CourseId: chapter.CourseId,
            TitleVi: chapter.TitleVi,
            DescriptionVi: chapter.DescriptionVi,
            SortOrder: chapter.SortOrder,
            IsActive: chapter.IsActive,
            LessonCount: chapter.Lessons?.Count ?? 0,
            ConcurrencyToken: chapter.ConcurrencyToken,
            CreatedAt: chapter.CreatedAt,
            CreatedById: chapter.CreatedById,
            UpdatedAt: chapter.UpdatedAt,
            UpdatedById: chapter.UpdatedById,
            DeletedAt: chapter.DeletedAt,
            DeletedById: chapter.DeletedById);
    }
}
