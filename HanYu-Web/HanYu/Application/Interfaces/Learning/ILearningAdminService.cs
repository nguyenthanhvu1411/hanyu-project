using HanYu.Application.Common.Models;
using HanYu.Application.Features.Learning.Admin.Activities;
using HanYu.Application.Features.Learning.Admin.Goals;
using HanYu.Application.Features.Learning.Admin.Summaries;

namespace HanYu.Application.Interfaces.Learning;

public interface ILearningAdminService
{
    Task<Result<AdminLearningGoalResponse>>
        CreateGoalAsync(
            CreateLearningGoalRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningGoalResponse>>
        GetGoalAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminLearningGoalResponse>>>
        GetGoalsAsync(
            AdminLearningGoalQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningGoalResponse>>
        UpdateGoalAsync(
            long id,
            UpdateLearningGoalRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteGoalAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLearningActivityResponse>>
        CreateActivityAsync(
            CreateLearningActivityRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningActivityResponse>>
        GetActivityAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminLearningActivityResponse>>>
        GetActivitiesAsync(
            AdminLearningActivityQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningActivityResponse>>
        UpdateActivityAsync(
            long id,
            UpdateLearningActivityRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteActivityAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLearningSummaryResponse>>
        GetSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminLearningSummaryResponse>>>
        GetSummariesAsync(
            AdminLearningSummaryQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningSummaryResponse>>
        UpdateSummaryAsync(
            Guid userId,
            UpdateLearningSummaryRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminLearningSummaryResponse>>
        RecomputeSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
