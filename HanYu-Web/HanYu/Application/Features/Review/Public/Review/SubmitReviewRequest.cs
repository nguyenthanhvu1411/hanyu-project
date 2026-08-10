using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Review;

public sealed record SubmitReviewRequest(
    Guid VocabularyPublicId,
    ReviewRating Rating,
    bool WasCorrect,
    int? ResponseTimeMs);
