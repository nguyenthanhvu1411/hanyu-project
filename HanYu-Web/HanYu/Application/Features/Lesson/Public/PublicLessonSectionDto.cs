using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Public;

public sealed record PublicLessonSectionDto(
    Guid PublicId,
    LessonSectionType Type,
    string? TitleVi,
    string? ContentVi,
    int SortOrder,
    bool IsRequired,
    int? EstimatedSeconds);
