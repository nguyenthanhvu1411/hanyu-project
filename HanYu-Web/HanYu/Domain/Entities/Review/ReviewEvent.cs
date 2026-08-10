using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Review;

public class ReviewEvent : BaseEntity
{
    public Guid UserId { get; private set; }

    public long VocabularyId { get; private set; }

    public long? FlashcardSessionId { get; private set; }

    public ReviewRating Rating { get; private set; }

    public bool WasCorrect { get; private set; }

    public int? ResponseTimeMs { get; private set; }

    public decimal MasteryBefore { get; private set; }

    public decimal MasteryAfter { get; private set; }

    public int? IntervalBeforeMinutes { get; private set; }

    public int IntervalAfterMinutes { get; private set; }

    public DateTimeOffset ReviewedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected ReviewEvent()
    {
    }

    public ReviewEvent(
        Guid userId,
        long vocabularyId,
        ReviewRating rating,
        bool wasCorrect,
        decimal masteryBefore,
        decimal masteryAfter,
        int intervalAfterMinutes,
        int? intervalBeforeMinutes = null,
        int? responseTimeMs = null,
        long? flashcardSessionId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        if (flashcardSessionId.HasValue &&
            flashcardSessionId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(flashcardSessionId));
        }

        ValidateMastery(
            masteryBefore,
            nameof(masteryBefore));

        ValidateMastery(
            masteryAfter,
            nameof(masteryAfter));

        if (intervalBeforeMinutes.HasValue &&
            intervalBeforeMinutes.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(intervalBeforeMinutes));
        }

        if (intervalAfterMinutes <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(intervalAfterMinutes));

        if (responseTimeMs.HasValue &&
            responseTimeMs.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(responseTimeMs));
        }

        UserId = userId;
        VocabularyId = vocabularyId;
        FlashcardSessionId = flashcardSessionId;

        Rating = rating;
        WasCorrect = wasCorrect;
        ResponseTimeMs = responseTimeMs;

        MasteryBefore = masteryBefore;
        MasteryAfter = masteryAfter;

        IntervalBeforeMinutes =
            intervalBeforeMinutes;

        IntervalAfterMinutes =
            intervalAfterMinutes;
    }

    private static void ValidateMastery(
        decimal value,
        string parameterName)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Mastery phải từ 0 đến 100.");
    }
}
