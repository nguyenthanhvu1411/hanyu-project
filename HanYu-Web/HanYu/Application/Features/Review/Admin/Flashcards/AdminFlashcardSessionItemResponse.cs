using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Flashcards;

public sealed record AdminFlashcardSessionItemResponse(
    long Id,
    Guid PublicId,
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    int SortOrder,
    bool IsAnswered,
    ReviewRating? Rating,
    bool? WasCorrect,
    int? ResponseTimeMs,
    DateTimeOffset? AnsweredAt);
