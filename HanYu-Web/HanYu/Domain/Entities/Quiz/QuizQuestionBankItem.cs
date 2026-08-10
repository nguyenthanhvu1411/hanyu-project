namespace HanYu.Domain.Entities.Quiz;

public class QuizQuestionBankItem
{
    public long QuestionBankId { get; private set; }
    public long QuestionId { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset AddedAt { get; private set; } = DateTimeOffset.UtcNow;
    public QuizQuestionBank QuestionBank { get; private set; } = null!;
    public QuizQuestion Question { get; private set; } = null!;
    
    protected QuizQuestionBankItem() { }

    public QuizQuestionBankItem(
        long questionBankId,
        long questionId,
        int sortOrder)
    {
        if (questionBankId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(questionBankId));

        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(questionId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        QuestionBankId =
            questionBankId;

        QuestionId =
            questionId;

        SortOrder =
            sortOrder;
    }

    public void ChangeOrder(
        int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder =
            sortOrder;
    }
}
