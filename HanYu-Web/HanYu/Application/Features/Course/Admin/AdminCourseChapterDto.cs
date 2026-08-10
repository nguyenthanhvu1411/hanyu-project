namespace HanYu.Application.Features.Course.Admin;

public sealed record AdminCourseChapterDto(
    long Id,
    Guid PublicId,
    string TitleVi,
    string? DescriptionVi,
    int SortOrder,
    bool IsActive,
    int LessonCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
