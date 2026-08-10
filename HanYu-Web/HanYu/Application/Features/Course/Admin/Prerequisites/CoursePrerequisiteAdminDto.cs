namespace HanYu.Application.Features.Course.Admin.Prerequisites;

public sealed record CoursePrerequisiteAdminDto(
    long Id,
    Guid PublicId,
    long CourseId,
    long RequiredCourseId,
    Guid RequiredCoursePublicId,
    string RequiredCourseCode,
    string RequiredCourseSlug,
    string RequiredCourseTitleVi,
    bool IsRequired,
    int SortOrder,
    Guid ConcurrencyToken,
    DateTimeOffset CreatedAt,
    Guid? CreatedById,
    DateTimeOffset UpdatedAt,
    Guid? UpdatedById,
    DateTimeOffset? DeletedAt,
    Guid? DeletedById);
