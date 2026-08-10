namespace HanYu.Application.Features.Lesson.Admin.Assets;

public sealed record UpdateLessonAssetRequest(
    string? Url,
    string? CaptionVi,
    long? AudioAssetId,
    int SortOrder);
