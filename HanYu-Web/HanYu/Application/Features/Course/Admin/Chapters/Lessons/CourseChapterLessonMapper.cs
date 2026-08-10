using LessonEntity = HanYu.Domain.Entities.Lesson.Lesson;

namespace HanYu.Application.Features.Course.Admin.Chapters.Lessons;

public static class CourseChapterLessonMapper
{
    public static CourseChapterLessonAdminDto ToDto(
        LessonEntity lesson)
    {
        ArgumentNullException.ThrowIfNull(
            lesson);

        return new CourseChapterLessonAdminDto(
            Id:
                lesson.Id,

            PublicId:
                lesson.PublicId,

            CourseChapterId:
                lesson.CourseChapterId,

            Slug:
                lesson.Slug,

            TitleVi:
                lesson.TitleVi,

            SortOrder:
                lesson.SortOrder,

            EstimatedMinutes:
                lesson.EstimatedMinutes,

            Difficulty:
                lesson.Difficulty,

            Status:
                lesson.Status,

            Version:
                lesson.Version,

            PublishedAt:
                lesson.PublishedAt,

            CreatedAt:
                lesson.CreatedAt,

            UpdatedAt:
                lesson.UpdatedAt);
    }
}
