using HanYu.Application.Common.Models;
using HanYu.Application.Features.Analytics.Admin.Dashboard;
using HanYu.Application.Features.Analytics.Admin.Users;
using HanYu.Application.Features.Analytics.Public.Me;

namespace HanYu.Application.Interfaces.Analytics;

public interface IAnalyticsAdminService
{
    Task<Result<AdminAnalyticsDashboardResponse>>
        GetDashboardAsync(
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminDailyLearningStatResponse>>>
        GetDailyStatsAsync(
            AdminLearningStatQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<MyLearningSummaryResponse>>
        GetUserSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
