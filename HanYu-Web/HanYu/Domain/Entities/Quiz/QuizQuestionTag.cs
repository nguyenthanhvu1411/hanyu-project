namespace HanYu.Domain.Entities.Quiz;

public class QuizQuestionTag
{
    public long QuestionId { get; private set; }

    public long TagId { get; private set; }

    public QuizQuestion Question { get; private set; } = null!;

    public QuizTag Tag { get; private set; } = null!;

    protected QuizQuestionTag()
    {
    }

    public QuizQuestionTag(
        long questionId,
        long tagId)
    {
        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(questionId));

        if (tagId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(tagId));

        QuestionId = questionId;
        TagId = tagId;
    }
}
