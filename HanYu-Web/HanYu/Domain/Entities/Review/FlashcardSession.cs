using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Review;

public class FlashcardSession : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public FlashcardMode Mode { get; private set; }

    public FlashcardSourceType SourceType { get; private set; }

    public long? SourceId { get; private set; }

    public FlashcardSessionStatus Status { get; private set; }
        = FlashcardSessionStatus.Active;

    public DateTimeOffset StartedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; private set; }

    public int CurrentIndex { get; private set; }

    public int TotalItems { get; private set; }

    public int CorrectItems { get; private set; }

    public ICollection<FlashcardSessionItem> Items { get; private set; }
        = new List<FlashcardSessionItem>();

    public int WrongItems =>
        Math.Max(0, CurrentIndex - CorrectItems);

    public decimal AccuracyPercent =>
        CurrentIndex == 0
            ? 0
            : Math.Round(
                CorrectItems * 100m / CurrentIndex,
                2);

    protected FlashcardSession()
    {
    }

    public FlashcardSession(
        Guid userId,
        FlashcardMode mode,
        FlashcardSourceType sourceType,
        int totalItems,
        long? sourceId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (totalItems <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(totalItems));

        if (sourceId.HasValue &&
            sourceId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sourceId));
        }

        UserId = userId;
        Mode = mode;
        SourceType = sourceType;
        SourceId = sourceId;
        TotalItems = totalItems;
    }

    public void RegisterAnswer(bool wasCorrect)
    {
        EnsureActive();

        if (CurrentIndex >= TotalItems)
            throw new InvalidOperationException(
                "Session đã xử lý hết flashcard.");

        CurrentIndex++;

        if (wasCorrect)
            CorrectItems++;

        if (CurrentIndex >= TotalItems)
        {
            Complete();
            return;
        }

        MarkUpdated();
    }

    public void Complete()
    {
        if (Status == FlashcardSessionStatus.Completed)
            return;

        if (Status == FlashcardSessionStatus.Abandoned)
            throw new InvalidOperationException(
                "Session đã Abandoned.");

        Status =
            FlashcardSessionStatus.Completed;

        CompletedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Abandon()
    {
        if (Status == FlashcardSessionStatus.Abandoned)
            return;

        if (Status == FlashcardSessionStatus.Completed)
            throw new InvalidOperationException(
                "Session đã Completed.");

        Status =
            FlashcardSessionStatus.Abandoned;

        CompletedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    private void EnsureActive()
    {
        if (Status != FlashcardSessionStatus.Active)
            throw new InvalidOperationException(
                "Flashcard session không còn Active.");
    }
}
