namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCourseDetailDto(
    Guid PublicId,
    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,
    string? DescriptionVi,

    string? HskCode,
    string? HskNameVi,

    string? CoverImageUrl,
    int? EstimatedMinutes,
    bool IsFeatured,

    IReadOnlyList<PublicCourseChapterDto> Chapters,
    IReadOnlyList<PublicCoursePrerequisiteDto> Prerequisites);
