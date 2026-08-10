namespace HanYu.Application.Features.Lesson.Public.Progress;

public sealed record SaveLessonProgressRequest(
    Guid? LastSectionPublicId,
    int LastPosition,
    decimal CompletionPercent);
