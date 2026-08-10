using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Assets;

public sealed record CreateLessonAssetRequest(
    LessonAssetType AssetType,
    string? Url,
    string? CaptionVi,
    long? AudioAssetId,
    int SortOrder);
