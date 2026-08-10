using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Course.Admin;

public sealed record AdminCourseDetailDto(
    long Id,
    Guid PublicId,
    string Code,
    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,
    string? DescriptionVi,

    long? HskLevelId,
    string? HskCode,
    string? HskNameVi,

    string? CoverImageUrl,
    int SortOrder,
    int? EstimatedMinutes,

    ContentStatus Status,
    bool IsActive,
    bool IsFeatured,

    DateTimeOffset? PublishedAt,
    Guid? PublishedById,

    DateTimeOffset? ArchivedAt,
    Guid? ArchivedById,

    Guid ConcurrencyToken,

    DateTimeOffset CreatedAt,
    Guid? CreatedById,

    DateTimeOffset UpdatedAt,
    Guid? UpdatedById,

    DateTimeOffset? DeletedAt,
    Guid? DeletedById,

    IReadOnlyList<AdminCourseChapterDto> Chapters,
    IReadOnlyList<AdminCoursePrerequisiteDto> Prerequisites);
