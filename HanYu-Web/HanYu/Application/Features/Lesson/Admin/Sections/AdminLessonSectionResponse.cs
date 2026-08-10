using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Sections;

public sealed record AdminLessonSectionResponse(
    long Id,
    Guid PublicId,
    long LessonId,
    LessonSectionType SectionType,
    string? TitleVi,
    string? ContentVi,
    int SortOrder,
    bool IsRequired,
    int? EstimatedSeconds,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
