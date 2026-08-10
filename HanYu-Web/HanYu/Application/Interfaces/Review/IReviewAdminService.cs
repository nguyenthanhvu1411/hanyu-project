using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.Dashboard;
using HanYu.Application.Features.Review.Admin.Events;
using HanYu.Application.Features.Review.Admin.Flashcards;
using HanYu.Application.Features.Review.Admin.States;
using HanYu.Application.Features.Review.Admin.Users;

namespace HanYu.Application.Interfaces.Review;

public interface IReviewAdminService
{
    Task<Result<AdminReviewDashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminVocabularyStateResponse>>> GetStatesAsync(AdminVocabularyStateQuery query, CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyStateDetailResponse>> GetStateAsync(Guid userId, long vocabularyId, CancellationToken cancellationToken = default);

    Task<Result> ResetStateAsync(Guid userId, long vocabularyId, AdminResetVocabularyStateRequest request, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminReviewEventResponse>>> GetEventsAsync(AdminReviewEventQuery query, CancellationToken cancellationToken = default);

    Task<Result<AdminReviewEventResponse>> GetEventAsync(long id, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminFlashcardSessionResponse>>> GetFlashcardSessionsAsync(AdminFlashcardSessionQuery query, CancellationToken cancellationToken = default);

    Task<Result<AdminFlashcardSessionDetailResponse>> GetFlashcardSessionAsync(long id, CancellationToken cancellationToken = default);

    Task<Result> AbandonFlashcardSessionAsync(long id, CancellationToken cancellationToken = default);

    Task<Result<AdminUserReviewSummaryResponse>> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}
