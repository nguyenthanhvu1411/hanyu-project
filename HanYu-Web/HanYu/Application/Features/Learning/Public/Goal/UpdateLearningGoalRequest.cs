using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Public.Goal;

public sealed record UpdateLearningGoalRequest(
    short TargetHskLevel,
    DateOnly? TargetDate,
    short DailyGoalMinutes,
    short? DailyVocabularyGoal,
    short? WeeklyLessonGoal);
