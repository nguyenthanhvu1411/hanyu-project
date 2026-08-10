using HanYu.Application.Features.Lesson.Public.Progress;

namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonDetailResponse(
    Guid PublicId,
    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,
    string? DescriptionVi,
    string? ObjectiveVi,
    string? CoverImageUrl,
    long HskLevel,
    string HskCode,
    string HskNameVi,
    short EstimatedMinutes,
    short Difficulty,
    bool IsFeatured,
    string? TopicSlug,
    string? TopicNameVi,
    bool IsLocked,
    bool IsBookmarked,
    LessonProgressResponse? Progress,
    IReadOnlyCollection<LessonSectionResponse> Sections,
    IReadOnlyCollection<LessonVocabularyResponse> Vocabulary,
    IReadOnlyCollection<LessonAssetResponse> Assets,
    IReadOnlyCollection<LessonPrerequisiteResponse> Prerequisites);
