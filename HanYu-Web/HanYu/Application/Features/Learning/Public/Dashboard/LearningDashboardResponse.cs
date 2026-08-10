using HanYu.Application.Features.Learning.Public.Goal;
using HanYu.Application.Features.Learning.Public.Summary;

namespace HanYu.Application.Features.Learning.Public.Dashboard;

public sealed record LearningDashboardResponse(
    LearningGoalResponse? Goal,
    LearningSummaryResponse Summary,
    int TodayLearningMinutes,
    int TodayXp,
    int TodayActivities,
    bool DailyGoalCompleted);
