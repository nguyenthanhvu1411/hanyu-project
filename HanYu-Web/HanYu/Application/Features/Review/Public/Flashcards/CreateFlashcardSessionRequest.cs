using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Flashcards;

public sealed record CreateFlashcardSessionRequest(
    FlashcardMode Mode,
    FlashcardSourceType SourceType,
    Guid? SourcePublicId,
    IReadOnlyCollection<Guid>? VocabularyPublicIds,
    int Limit = 20);
