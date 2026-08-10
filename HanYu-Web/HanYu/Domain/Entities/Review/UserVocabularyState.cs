using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Review;

public class UserVocabularyState
{
    public Guid UserId { get; private set; }

    public long VocabularyId { get; private set; }

    public LearningState LearningState { get; private set; }
        = LearningState.NotStarted;

    public bool IsFavorite { get; private set; }

    public decimal MasteryScore { get; private set; }

    public int CorrectCount { get; private set; }

    public int WrongCount { get; private set; }

    public int ConsecutiveCorrect { get; private set; }

    public int DistinctCorrectDays { get; private set; }

    public DateTimeOffset? LastCorrectAt { get; private set; }

    public DateTimeOffset? LastReviewedAt { get; private set; }

    public DateTimeOffset? NextReviewAt { get; private set; }

    public int? CurrentIntervalMinutes { get; private set; }

    public DateTimeOffset? FirstLearnedAt { get; private set; }

    public DateTimeOffset? MasteredAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public User User { get; private set; } = null!;

    public HanYu.Domain.Entities.Vocabulary.Vocabulary Vocabulary { get; private set; } = null!;

    protected UserVocabularyState()
    {
    }

    public UserVocabularyState(
        Guid userId,
        long vocabularyId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        UserId = userId;
        VocabularyId = vocabularyId;
    }

    public void MarkFavorite()
    {
        if (IsFavorite)
            return;

        IsFavorite = true;

        MarkUpdated();
    }

    public void Unfavorite()
    {
        if (!IsFavorite)
            return;

        IsFavorite = false;

        MarkUpdated();
    }

    public void ApplyReview(
        ReviewRating rating,
        bool wasCorrect,
        decimal masteryAfter,
        int nextIntervalMinutes,
        DateTimeOffset reviewedAt,
        DateTimeOffset nextReviewAt,
        DateOnly localLearningDate)
    {
        if (reviewedAt > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new ArgumentOutOfRangeException(
                nameof(reviewedAt));

        if (masteryAfter < 0 ||
            masteryAfter > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(masteryAfter));
        }

        if (nextIntervalMinutes <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(nextIntervalMinutes));

        if (nextReviewAt <= reviewedAt)
            throw new ArgumentOutOfRangeException(
                nameof(nextReviewAt));

        if (LastReviewedAt.HasValue &&
            reviewedAt < LastReviewedAt.Value)
        {
            throw new InvalidOperationException(
                "ReviewedAt không thể nhỏ hơn lần review gần nhất.");
        }

        if (wasCorrect)
        {
            CorrectCount++;
            ConsecutiveCorrect++;

            var isNewCorrectDay =
                !LastCorrectAt.HasValue ||
                DateOnly.FromDateTime(
                    LastCorrectAt.Value.DateTime)
                != localLearningDate;

            if (isNewCorrectDay)
                DistinctCorrectDays++;

            LastCorrectAt =
                reviewedAt;
        }
        else
        {
            WrongCount++;
            ConsecutiveCorrect = 0;
        }

        MasteryScore =
            masteryAfter;

        LastReviewedAt =
            reviewedAt;

        NextReviewAt =
            nextReviewAt;

        CurrentIntervalMinutes =
            nextIntervalMinutes;

        if (LearningState == LearningState.NotStarted)
        {
            LearningState =
                LearningState.Learning;

            FirstLearnedAt =
                reviewedAt;
        }

        EvaluateLearningState(
            masteryAfter,
            reviewedAt);

        MarkUpdated();
    }

    public void MarkKnown()
    {
        if (LearningState == LearningState.Mastered)
            return;

        LearningState =
            LearningState.Known;

        FirstLearnedAt ??=
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void MarkMastered()
    {
        if (LearningState == LearningState.Mastered)
            return;

        LearningState =
            LearningState.Mastered;

        MasteredAt =
            DateTimeOffset.UtcNow;

        FirstLearnedAt ??=
            MasteredAt;

        if (MasteryScore < 80)
            MasteryScore = 80;

        MarkUpdated();
    }

    public void ResetProgress()
    {
        LearningState =
            LearningState.NotStarted;

        MasteryScore = 0;

        CorrectCount = 0;
        WrongCount = 0;
        ConsecutiveCorrect = 0;
        DistinctCorrectDays = 0;

        LastCorrectAt = null;
        LastReviewedAt = null;
        NextReviewAt = null;

        CurrentIntervalMinutes = null;

        FirstLearnedAt = null;
        MasteredAt = null;

        MarkUpdated();
    }

    public void StartLearning(
        DateTimeOffset learnedAt,
        int initialIntervalMinutes)
    {
        if (initialIntervalMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialIntervalMinutes));
        }

        if (LearningState !=
            LearningState.NotStarted)
        {
            return;
        }

        LearningState =
            LearningState.Learning;

        FirstLearnedAt =
            learnedAt;

        CurrentIntervalMinutes =
            initialIntervalMinutes;

        NextReviewAt =
            learnedAt.AddMinutes(
                initialIntervalMinutes);

        MarkUpdated();
    }

    private void EvaluateLearningState(
        decimal masteryAfter,
        DateTimeOffset reviewedAt)
    {
        if (masteryAfter >= 80 &&
            DistinctCorrectDays >= 3)
        {
            LearningState =
                LearningState.Mastered;

            MasteredAt ??=
                reviewedAt;

            return;
        }

        if (masteryAfter >= 60)
        {
            LearningState =
                LearningState.Known;

            return;
        }

        LearningState =
            LearningState.Learning;

        MasteredAt = null;
    }

    private void MarkUpdated()
    {
        UpdatedAt =
            DateTimeOffset.UtcNow;
    }
}
