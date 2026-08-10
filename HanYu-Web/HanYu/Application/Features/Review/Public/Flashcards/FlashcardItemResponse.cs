using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Flashcards;

public sealed record FlashcardItemResponse(
    Guid PublicId,
    Guid VocabularyPublicId,
    int SortOrder,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    FlashcardMode Mode,
    bool IsAnswered,
    ReviewRating? Rating,
    bool? WasCorrect,
    int? ResponseTimeMs);
