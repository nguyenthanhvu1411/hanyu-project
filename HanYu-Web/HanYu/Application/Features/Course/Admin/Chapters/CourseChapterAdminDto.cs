namespace HanYu.Application.Features.Course.Admin.Chapters;

public sealed record CourseChapterAdminDto(
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
    Guid? CreatedById,
    DateTimeOffset UpdatedAt,
    Guid? UpdatedById,
    DateTimeOffset? DeletedAt,
    Guid? DeletedById);
