using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Flashcards;

public sealed record FlashcardSessionResponse(
    Guid PublicId,
    FlashcardMode Mode,
    FlashcardSourceType SourceType,
    FlashcardSessionStatus Status,
    int CurrentIndex,
    int TotalItems,
    int CorrectItems,
    int WrongItems,
    decimal AccuracyPercent,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyCollection<FlashcardItemResponse> Items);
