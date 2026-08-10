namespace HanYu.Application.Features.Course.Admin;

public sealed record AdminCourseChapterDto(
    long Id,
    Guid PublicId,
    long CourseId,
    string TitleVi,
    string? DescriptionVi,
    int SortOrder,
    bool IsActive,
    int LessonCount,
    Guid ConcurrencyToken,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DeletedAt,
    Guid? DeletedById);
