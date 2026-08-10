using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Assets;

public sealed record AdminLessonAssetResponse(
    long Id,
    Guid PublicId,
    long LessonId,
    long? AudioAssetId,
    LessonAssetType AssetType,
    string? Url,
    string? CaptionVi,
    int SortOrder,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
