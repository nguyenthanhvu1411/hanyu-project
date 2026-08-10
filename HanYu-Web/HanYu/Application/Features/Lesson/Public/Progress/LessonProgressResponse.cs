using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Public.Progress;

public sealed record LessonProgressResponse(
    LessonProgressStatus Status,
    Guid? LastSectionPublicId,
    int LastPosition,
    decimal CompletionPercent,
    DateTimeOffset? StartedAt,
    DateTimeOffset? LastAccessedAt,
    DateTimeOffset? CompletedAt);
