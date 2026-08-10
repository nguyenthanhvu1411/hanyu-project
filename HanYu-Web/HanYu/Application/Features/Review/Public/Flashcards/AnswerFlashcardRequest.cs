using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Flashcards;

public sealed record AnswerFlashcardRequest(
    ReviewRating Rating,
    bool WasCorrect,
    int? ResponseTimeMs);
