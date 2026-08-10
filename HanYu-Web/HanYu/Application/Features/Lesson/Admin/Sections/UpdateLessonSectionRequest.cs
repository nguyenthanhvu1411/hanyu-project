using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Sections;

public sealed record UpdateLessonSectionRequest(
    LessonSectionType SectionType,
    string? TitleVi,
    string? ContentVi,
    int SortOrder,
    bool IsRequired,
    int? EstimatedSeconds);
