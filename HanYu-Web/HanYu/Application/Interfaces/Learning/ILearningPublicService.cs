using HanYu.Application.Common.Models;
using HanYu.Application.Features.Learning.Public.Activities;
using HanYu.Application.Features.Learning.Public.Dashboard;
using HanYu.Application.Features.Learning.Public.Goal;
using HanYu.Application.Features.Learning.Public.Summary;

namespace HanYu.Application.Interfaces.Learning;

public interface ILearningPublicService
{
    Task<Result<LearningGoalResponse>>
        GetMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<LearningGoalResponse>>
        UpdateMyGoalAsync(
            Guid userId,
            UpdateLearningGoalRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<LearningGoalResponse>>
        PauseMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<LearningGoalResponse>>
        ResumeMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<LearningActivityResponse>>>
        GetMyActivitiesAsync(
            Guid userId,
            LearningActivityQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<LearningActivityResponse>>
        GetMyActivityAsync(
            Guid userId,
            long activityId,
            CancellationToken cancellationToken = default);

    Task<Result<LearningSummaryResponse>>
        GetMySummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<LearningDashboardResponse>>
        GetDashboardAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
