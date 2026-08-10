using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Quiz;

public class QuizAttempt : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public long QuizId { get; private set; }

    public int AttemptNumber { get; private set; }

    public string IdempotencyKey { get; private set; }
        = string.Empty;

    public QuizAttemptStatus Status { get; private set; }
        = QuizAttemptStatus.InProgress;

    public decimal? Score { get; private set; }

    public decimal? MaxScore { get; private set; }

    public decimal? Percentage { get; private set; }

    public bool? IsPassed { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? SubmittedAt { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public int? DurationSeconds { get; private set; }

    public int CorrectAnswers { get; private set; }

    public int WrongAnswers { get; private set; }

    public int UnansweredQuestions { get; private set; }

    public User User { get; private set; } = null!;

    public Quiz Quiz { get; private set; } = null!;

    public ICollection<QuizAttemptAnswer> Answers { get; private set; }
        = new List<QuizAttemptAnswer>();

    public ICollection<QuizAttemptQuestion> Questions { get; private set; }
        = new List<QuizAttemptQuestion>();

    protected QuizAttempt()
    {
    }

    public QuizAttempt(
        Guid userId,
        long quizId,
        int attemptNumber,
        string idempotencyKey,
        DateTimeOffset? expiresAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (quizId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quizId));

        if (attemptNumber <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(attemptNumber));

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException(
                "IdempotencyKey không được để trống.",
                nameof(idempotencyKey));

        UserId = userId;
        QuizId = quizId;
        AttemptNumber = attemptNumber;

        IdempotencyKey =
            idempotencyKey.Trim();

        StartedAt =
            DateTimeOffset.UtcNow;

        ExpiresAt =
            expiresAt;
    }

    public void Submit(
        decimal score,
        decimal maxScore,
        decimal passingScore,
        int correctAnswers,
        int wrongAnswers,
        int unansweredQuestions)
    {
        EnsureInProgress();

        if (score < 0)
            throw new ArgumentOutOfRangeException(nameof(score));

        if (maxScore <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxScore));

        if (passingScore < 0 || passingScore > 100)
            throw new ArgumentOutOfRangeException(nameof(passingScore));

        if (correctAnswers < 0 ||
            wrongAnswers < 0 ||
            unansweredQuestions < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(correctAnswers));
        }

        var now =
            DateTimeOffset.UtcNow;

        Score =
            score;

        MaxScore =
            maxScore;

        Percentage =
            Math.Round(
                score * 100m / maxScore,
                2);

        IsPassed =
            Percentage >= passingScore;

        CorrectAnswers =
            correctAnswers;

        WrongAnswers =
            wrongAnswers;

        UnansweredQuestions =
            unansweredQuestions;

        Status =
            QuizAttemptStatus.Submitted;

        SubmittedAt =
            now;

        DurationSeconds =
            Math.Max(
                0,
                (int)(now - StartedAt)
                    .TotalSeconds);

        MarkUpdated();
    }

    public void Expire()
    {
        if (Status != QuizAttemptStatus.InProgress)
            return;

        Status =
            QuizAttemptStatus.Expired;

        DurationSeconds =
            Math.Max(
                0,
                (int)(DateTimeOffset.UtcNow - StartedAt)
                    .TotalSeconds);

        MarkUpdated();
    }

    public bool IsExpired(
        DateTimeOffset now)
    {
        return ExpiresAt.HasValue &&
               ExpiresAt.Value <= now;
    }

    private void EnsureInProgress()
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            throw new InvalidOperationException(
                "QuizAttempt không còn InProgress.");
        }

        if (IsExpired(DateTimeOffset.UtcNow))
        {
            throw new InvalidOperationException(
                "QuizAttempt đã hết thời gian.");
        }
    }
}
