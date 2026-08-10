namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonListItemResponse(
    Guid PublicId,
    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,
    string? CoverImageUrl,
    long HskLevel,
    string HskCode,
    short EstimatedMinutes,
    short Difficulty,
    bool IsFeatured,
    string? TopicSlug,
    string? TopicNameVi);
