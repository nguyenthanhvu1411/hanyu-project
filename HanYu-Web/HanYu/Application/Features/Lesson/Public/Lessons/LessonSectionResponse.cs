using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonSectionResponse(
    Guid PublicId,
    LessonSectionType SectionType,
    string? TitleVi,
    string? ContentVi,
    int SortOrder,
    bool IsRequired,
    int? EstimatedSeconds);
