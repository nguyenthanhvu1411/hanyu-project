namespace HanYu.Application.Features.Review.Public.Favorites;

public sealed record FavoriteVocabularyResponse(
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    decimal MasteryScore,
    DateTimeOffset? NextReviewAt);
