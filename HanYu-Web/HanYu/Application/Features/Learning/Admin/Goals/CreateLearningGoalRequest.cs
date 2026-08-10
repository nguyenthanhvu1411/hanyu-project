namespace HanYu.Application.Features.Learning.Admin.Goals;

public sealed record CreateLearningGoalRequest(
    Guid UserId,
    short TargetHskLevel,
    DateOnly? TargetDate,
    short DailyGoalMinutes,
    short? DailyVocabularyGoal,
    short? WeeklyLessonGoal);
