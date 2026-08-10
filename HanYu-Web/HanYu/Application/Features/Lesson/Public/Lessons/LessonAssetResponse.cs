using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonAssetResponse(
    Guid PublicId,
    LessonAssetType AssetType,
    string? Url,
    string? CaptionVi,
    int SortOrder);
