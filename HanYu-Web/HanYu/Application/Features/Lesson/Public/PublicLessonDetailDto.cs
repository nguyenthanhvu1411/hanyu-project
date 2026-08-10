namespace HanYu.Application.Features.Lesson.Public;

public sealed record PublicLessonDetailDto(
    Guid PublicId,

    string Slug,
    string TitleVi,

    string? ShortDescriptionVi,
    string? DescriptionVi,
    string? ObjectiveVi,

    string? CoverImageUrl,

    string HskCode,
    string HskNameVi,

    short EstimatedMinutes,
    short Difficulty,

    PublicLessonContextDto? Context,

    IReadOnlyList<PublicLessonSectionDto> Sections);
