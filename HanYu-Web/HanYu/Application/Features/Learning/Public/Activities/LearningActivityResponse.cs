using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Public.Activities;

public sealed record LearningActivityResponse(
    LearningActivityType ActivityType,
    int DurationSeconds,
    int XpEarned,
    bool IsCompleted,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt);
