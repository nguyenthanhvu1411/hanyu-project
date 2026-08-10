namespace HanYu.Application.Features.Course.Admin;

public sealed record AdminCoursePrerequisiteDto(
    long Id,
    long RequiredCourseId,
    Guid RequiredCoursePublicId,
    string RequiredCourseCode,
    string RequiredCourseTitleVi,
    bool IsRequired,
    int SortOrder);
