using HanYu.Application.Features.Review.Public.Review;

namespace HanYu.Application.Features.Review.Public.Flashcards;

public sealed record FlashcardAnswerResponse(
    FlashcardSessionResponse Session,
    ReviewResultResponse Review);
