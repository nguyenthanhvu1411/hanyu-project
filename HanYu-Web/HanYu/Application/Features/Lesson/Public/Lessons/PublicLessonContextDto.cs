namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record PublicLessonContextDto(
    Guid CoursePublicId,
    string CourseSlug,
    string CourseTitleVi,
    Guid ChapterPublicId,
    string ChapterTitleVi);
