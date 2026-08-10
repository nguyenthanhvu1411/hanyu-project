using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Goals;

public sealed record AdminLearningGoalResponse(
    long Id,
    Guid UserId,
    short TargetHskLevel,
    DateOnly? TargetDate,
    short DailyGoalMinutes,
    short? DailyVocabularyGoal,
    short? WeeklyLessonGoal,
    LearningGoalStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? PausedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
