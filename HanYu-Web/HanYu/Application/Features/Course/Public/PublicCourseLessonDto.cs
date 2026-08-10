namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCourseLessonDto(
    Guid PublicId,
    string Slug,
    string TitleVi,
    int SortOrder,
    int? EstimatedMinutes);
