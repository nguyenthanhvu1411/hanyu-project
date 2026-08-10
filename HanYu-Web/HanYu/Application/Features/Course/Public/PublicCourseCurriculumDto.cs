namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCourseCurriculumDto(
    Guid CoursePublicId,
    string Slug,
    string TitleVi,
    int ChapterCount,
    int LessonCount,
    int? EstimatedMinutes,
    IReadOnlyList<PublicCourseChapterDto> Chapters);
