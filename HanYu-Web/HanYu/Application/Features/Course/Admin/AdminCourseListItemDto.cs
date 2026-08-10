using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Course.Admin;

public sealed record AdminCourseListItemDto(
    long Id,
    Guid PublicId,
    string Code,
    string Slug,
    string TitleVi,
    long? HskLevelId,
    string? HskCode,
    string? HskNameVi,
    string? CoverImageUrl,
    int SortOrder,
    int? EstimatedMinutes,
    ContentStatus Status,
    bool IsActive,
    bool IsFeatured,
    int ChapterCount,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
