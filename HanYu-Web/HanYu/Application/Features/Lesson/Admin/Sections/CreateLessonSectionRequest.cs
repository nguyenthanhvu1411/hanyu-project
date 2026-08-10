using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Sections;

public sealed record CreateLessonSectionRequest(
    LessonSectionType SectionType,
    int SortOrder,
    string? TitleVi,
    string? ContentVi,
    bool IsRequired,
    int? EstimatedSeconds);
