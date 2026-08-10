namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCourseListItemDto(
    Guid PublicId,
    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,
    string? HskCode,
    string? HskNameVi,
    string? CoverImageUrl,
    int? EstimatedMinutes,
    bool IsFeatured,
    int ChapterCount);
