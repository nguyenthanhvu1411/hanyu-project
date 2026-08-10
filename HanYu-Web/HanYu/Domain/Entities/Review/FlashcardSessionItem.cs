using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Review;

public class FlashcardSessionItem : BaseEntity
{
    public long FlashcardSessionId { get; private set; }

    public long VocabularyId { get; private set; }

    public int SortOrder { get; private set; }

    public ReviewRating? Rating { get; private set; }

    public bool? WasCorrect { get; private set; }

    public int? ResponseTimeMs { get; private set; }

    public DateTimeOffset? AnsweredAt { get; private set; }

    public FlashcardSession Session { get; private set; }
        = null!;

    public bool IsAnswered =>
        AnsweredAt.HasValue;

    protected FlashcardSessionItem()
    {
    }

    public FlashcardSessionItem(
        long flashcardSessionId,
        long vocabularyId,
        int sortOrder)
    {
        if (flashcardSessionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(flashcardSessionId));

        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        FlashcardSessionId =
            flashcardSessionId;

        VocabularyId =
            vocabularyId;

        SortOrder =
            sortOrder;
    }

    public void Answer(
        ReviewRating rating,
        bool wasCorrect,
        int? responseTimeMs = null)
    {
        if (IsAnswered)
            throw new InvalidOperationException(
                "Flashcard item đã được trả lời.");

        if (responseTimeMs.HasValue &&
            responseTimeMs.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(responseTimeMs));
        }

        Rating = rating;
        WasCorrect = wasCorrect;
        ResponseTimeMs = responseTimeMs;
        AnsweredAt = DateTimeOffset.UtcNow;
    }
}
