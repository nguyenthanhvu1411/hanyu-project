namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCourseChapterDto(
    Guid PublicId,
    string TitleVi,
    string? DescriptionVi,
    int SortOrder,
    int LessonCount,
    IReadOnlyList<PublicCourseLessonDto> Lessons);
