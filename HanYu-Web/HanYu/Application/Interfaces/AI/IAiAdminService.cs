using HanYu.Application.Common.Models;
using HanYu.Application.Features.AI.Admin.Cache;
using HanYu.Application.Features.AI.Admin.Conversations;
using HanYu.Application.Features.AI.Admin.Feedback;
using HanYu.Application.Features.AI.Admin.Requests;

namespace HanYu.Application.Interfaces.AI;

public interface IAiAdminService
{
    Task<Result<AdminAiDashboardResponse>>
        GetDashboardAsync(
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminAiRequestResponse>>>
        GetRequestsAsync(
            AdminAiRequestQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminAiRequestResponse>>
        GetRequestAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result> CancelRequestAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminAiConversationResponse>>>
        GetConversationsAsync(
            AdminAiConversationQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminAiConversationResponse>>
        GetConversationAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminAiFeedbackResponse>>>
        GetFeedbacksAsync(
            AdminAiFeedbackQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminAiCacheResponse>>>
        GetCacheAsync(
            AdminAiCacheQuery query,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteExpiredCacheEntryAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> DeleteExpiredCacheAsync(
        CancellationToken cancellationToken = default);
}
