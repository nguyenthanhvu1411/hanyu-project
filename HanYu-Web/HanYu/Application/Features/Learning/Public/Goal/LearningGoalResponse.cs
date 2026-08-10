using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Public.Goal;

public sealed record LearningGoalResponse(
    short TargetHskLevel,
    DateOnly? TargetDate,
    short DailyGoalMinutes,
    short? DailyVocabularyGoal,
    short? WeeklyLessonGoal,
    LearningGoalStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? PausedAt);
