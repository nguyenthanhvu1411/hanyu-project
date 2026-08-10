using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Goals;

public sealed record UpdateLearningGoalRequest(
    short TargetHskLevel,
    DateOnly? TargetDate,
    short DailyGoalMinutes,
    short? DailyVocabularyGoal,
    short? WeeklyLessonGoal,
    LearningGoalStatus Status);
