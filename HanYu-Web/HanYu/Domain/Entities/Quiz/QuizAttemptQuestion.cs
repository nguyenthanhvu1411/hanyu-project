using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizAttemptQuestion : BaseEntity
{
    public long AttemptId { get; private set; }

    public long QuestionId { get; private set; }

    public int SortOrder { get; private set; }

    public string QuestionSnapshotJson { get; private set; }
        = string.Empty;

    public QuizAttempt Attempt { get; private set; } = null!;

    public QuizQuestion Question { get; private set; } = null!;

    protected QuizAttemptQuestion()
    {
    }

    public QuizAttemptQuestion(
        long attemptId,
        long questionId,
        int sortOrder,
        string questionSnapshotJson)
    {
        if (attemptId < 0)
            throw new ArgumentOutOfRangeException(nameof(attemptId));

        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(questionId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder));

        if (string.IsNullOrWhiteSpace(questionSnapshotJson))
        {
            throw new ArgumentException(
                "QuestionSnapshotJson không được để trống.",
                nameof(questionSnapshotJson));
        }

        AttemptId =
            attemptId;

        QuestionId =
            questionId;

        SortOrder =
            sortOrder;

        QuestionSnapshotJson =
            questionSnapshotJson;
    }
}
