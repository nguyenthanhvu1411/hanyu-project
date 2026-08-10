namespace HanYu.Application.Features.Lesson.Public;

public sealed record PublicLessonContextDto(
    Guid CoursePublicId,
    string CourseSlug,
    string CourseTitleVi,
    Guid ChapterPublicId,
    string ChapterTitleVi);
