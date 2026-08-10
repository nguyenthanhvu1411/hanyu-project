using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizAttemptAnswer : TimestampedEntity
{
    public long AttemptId { get; private set; }

    public long QuestionId { get; private set; }

    public long? SelectedOptionId { get; private set; }

    public string? AnswerText { get; private set; }

    public string? AnswerJson { get; private set; }

    public bool? IsCorrect { get; private set; }

    public decimal? EarnedPoints { get; private set; }

    public int? ResponseTimeMs { get; private set; }

    public DateTimeOffset? AnsweredAt { get; private set; }

    public QuizAttempt Attempt { get; private set; } = null!;

    public QuizQuestion Question { get; private set; } = null!;

    public QuizQuestionOption? SelectedOption { get; private set; }

    protected QuizAttemptAnswer()
    {
    }

    public QuizAttemptAnswer(
        long attemptId,
        long questionId)
    {
        if (attemptId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attemptId));

        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(questionId));

        AttemptId =
            attemptId;

        QuestionId =
            questionId;
    }

    public void Answer(
        long? selectedOptionId,
        string? answerText,
        string? answerJson,
        bool isCorrect,
        decimal earnedPoints,
        int? responseTimeMs)
    {
        if (AnsweredAt.HasValue)
        {
            throw new InvalidOperationException(
                "Question đã được trả lời.");
        }

        if (selectedOptionId.HasValue &&
            selectedOptionId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(selectedOptionId));
        }

        if (earnedPoints < 0)
            throw new ArgumentOutOfRangeException(nameof(earnedPoints));

        if (responseTimeMs.HasValue &&
            responseTimeMs.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(responseTimeMs));
        }

        SelectedOptionId =
            selectedOptionId;

        AnswerText =
            Normalize(answerText);

        AnswerJson =
            Normalize(answerJson);

        IsCorrect =
            isCorrect;

        EarnedPoints =
            earnedPoints;

        ResponseTimeMs =
            responseTimeMs;

        AnsweredAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
