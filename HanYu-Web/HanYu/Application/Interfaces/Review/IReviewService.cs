using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Public.Favorites;
using HanYu.Application.Features.Review.Public.Queue;
using HanYu.Application.Features.Review.Public.Review;

namespace HanYu.Application.Interfaces.Review;

public interface IReviewService
{
    Task<Result<IReadOnlyCollection<ReviewQueueItemResponse>>>
        GetQueueAsync(
            Guid userId,
            ReviewQueueQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<ReviewQueueSummaryResponse>>
        GetSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<VocabularyLearningStateResponse>>
        GetStateAsync(
            Guid userId,
            Guid vocabularyPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<ReviewResultResponse>>
        SubmitReviewAsync(
            Guid userId,
            SubmitReviewRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> FavoriteAsync(
        Guid userId,
        Guid vocabularyPublicId,
        CancellationToken cancellationToken = default);

    Task<Result> UnfavoriteAsync(
        Guid userId,
        Guid vocabularyPublicId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<FavoriteVocabularyResponse>>>
        GetFavoritesAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
