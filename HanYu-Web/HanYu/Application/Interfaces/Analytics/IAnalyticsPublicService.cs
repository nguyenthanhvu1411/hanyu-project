using HanYu.Application.Common.Models;
using HanYu.Application.Features.Analytics.Public.Me;

namespace HanYu.Application.Interfaces.Analytics;

public interface IAnalyticsPublicService
{
    Task<Result<IReadOnlyCollection<MyLearningStatResponse>>>
        GetMyStatsAsync(
            Guid userId,
            DateOnly? from,
            DateOnly? to,
            CancellationToken cancellationToken = default);

    Task<Result<MyLearningSummaryResponse>>
        GetMySummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
