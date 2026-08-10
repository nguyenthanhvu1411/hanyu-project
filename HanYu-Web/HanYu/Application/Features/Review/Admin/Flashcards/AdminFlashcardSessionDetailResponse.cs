using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Flashcards;

public sealed record AdminFlashcardSessionDetailResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    FlashcardMode Mode,
    FlashcardSourceType SourceType,
    long? SourceId,
    FlashcardSessionStatus Status,
    int CurrentIndex,
    int TotalItems,
    int CorrectItems,
    int WrongItems,
    decimal AccuracyPercent,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyCollection<AdminFlashcardSessionItemResponse> Items);
