using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonSectionMediaResponse(
    Guid PublicId,
    Guid AssetPublicId,
    LessonAssetType AssetType,
    string? Url,
    string? CaptionVi,
    int SortOrder,
    bool IsRequired);

public sealed record LessonSectionResponse(
    Guid PublicId,
    LessonSectionType SectionType,
    string? TitleVi,
    string? ContentVi,
    int SortOrder,
    bool IsRequired,
    int? EstimatedSeconds,
    IReadOnlyCollection<LessonSectionMediaResponse> Media);
