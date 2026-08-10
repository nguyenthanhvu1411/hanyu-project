namespace HanYu.Application.Features.Lesson.Public.Progress;

public sealed record SaveSectionProgressRequest(
    int TimeSpentSeconds,
    bool IsCompleted);
