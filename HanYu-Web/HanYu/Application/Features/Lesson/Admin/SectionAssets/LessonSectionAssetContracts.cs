namespace HanYu.Application.Features.Lesson.Admin.SectionAssets;

public sealed record AdminLessonSectionAssetResponse(
    long Id,
    Guid PublicId,
    long LessonSectionId,
    long LessonAssetId,
    int SortOrder,
    string? CaptionVi,
    bool IsRequired,
    string AssetType,
    string? Url,
    long? AudioAssetId,
    string? AssetCaptionVi,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record AttachLessonSectionAssetRequest(
    long LessonAssetId,
    int SortOrder = 0,
    string? CaptionVi = null,
    bool IsRequired = false);

public sealed record UpdateLessonSectionAssetRequest(
    int SortOrder,
    string? CaptionVi,
    bool IsRequired);
