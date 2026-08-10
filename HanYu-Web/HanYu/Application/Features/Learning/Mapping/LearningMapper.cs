using HanYu.Application.Features.Learning.Admin.Activities;
using HanYu.Application.Features.Learning.Admin.Goals;
using HanYu.Application.Features.Learning.Admin.Summaries;
using HanYu.Application.Features.Learning.Public.Activities;
using HanYu.Application.Features.Learning.Public.Goal;
using HanYu.Application.Features.Learning.Public.Summary;
using HanYu.Domain.Entities.Learning;

namespace HanYu.Application.Features.Learning.Mapping;

public static class LearningMapper
{
    // Goals
    public static AdminLearningGoalResponse ToAdminGoalResponse(UserLearningGoal goal)
    {
        return new AdminLearningGoalResponse(
            goal.Id,
            goal.UserId,
            goal.TargetHskLevel,
            goal.TargetDate,
            goal.DailyGoalMinutes,
            goal.DailyVocabularyGoal,
            goal.WeeklyLessonGoal,
            goal.Status,
            goal.StartedAt,
            goal.CompletedAt,
            goal.PausedAt,
            goal.CreatedAt,
            goal.UpdatedAt);
    }

    public static LearningGoalResponse ToPublicGoalResponse(UserLearningGoal goal)
    {
        return new LearningGoalResponse(
            goal.TargetHskLevel,
            goal.TargetDate,
            goal.DailyGoalMinutes,
            goal.DailyVocabularyGoal,
            goal.WeeklyLessonGoal,
            goal.Status,
            goal.StartedAt,
            goal.CompletedAt,
            goal.PausedAt);
    }

    // Activities
    public static AdminLearningActivityResponse ToAdminActivityResponse(LearningActivity activity)
    {
        return new AdminLearningActivityResponse(
            activity.Id,
            activity.UserId,
            activity.ActivityType,
            activity.LessonId,
            activity.VocabularyId,
            activity.QuizAttemptId,
            activity.FlashcardSessionId,
            activity.DurationSeconds,
            activity.XpEarned,
            activity.IsCompleted,
            activity.MetadataJson,
            activity.StartedAt,
            activity.CompletedAt);
    }

    public static LearningActivityResponse ToPublicActivityResponse(LearningActivity activity)
    {
        return new LearningActivityResponse(
            activity.ActivityType,
            activity.DurationSeconds,
            activity.XpEarned,
            activity.IsCompleted,
            activity.StartedAt,
            activity.CompletedAt);
    }

    // Summaries
    public static AdminLearningSummaryResponse ToAdminSummaryResponse(UserLearningSummary summary)
    {
        return new AdminLearningSummaryResponse(
            summary.UserId,
            summary.TotalLearningSeconds,
            summary.TotalLessonsCompleted,
            summary.TotalVocabularyLearned,
            summary.TotalVocabularyMastered,
            summary.TotalReviews,
            summary.TotalQuizAttempts,
            summary.TotalQuizPassed,
            summary.TotalXp,
            summary.CurrentHskLevel,
            summary.OverallMasteryPercent,
            summary.LastLearningAt,
            summary.UpdatedAt);
    }

    public static LearningSummaryResponse ToPublicSummaryResponse(UserLearningSummary summary)
    {
        return new LearningSummaryResponse(
            summary.TotalLearningSeconds / 60, // TotalLearningMinutes
            summary.TotalLessonsCompleted,
            summary.TotalVocabularyLearned,
            summary.TotalVocabularyMastered,
            summary.TotalReviews,
            summary.TotalQuizAttempts,
            summary.TotalQuizPassed,
            summary.TotalXp,
            summary.CurrentHskLevel,
            summary.OverallMasteryPercent,
            summary.LastLearningAt);
    }
}
