namespace HanYu.Application.Features.Lesson.Public.Progress;

public sealed record LessonSectionProgressResponse(
    Guid SectionPublicId,
    bool IsCompleted,
    int TimeSpentSeconds,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt);
