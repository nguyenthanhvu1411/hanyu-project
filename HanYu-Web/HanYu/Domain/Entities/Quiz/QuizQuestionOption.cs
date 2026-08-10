using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizQuestionOption : AuditableEntity
{
    public long QuestionId { get; private set; }

    public string OptionText { get; private set; }
        = string.Empty;

    public string? OptionPinyin { get; private set; }

    public bool IsCorrect { get; private set; }

    public short SortOrder { get; private set; }

    public string? ExplanationVi { get; private set; }

    public QuizQuestion Question { get; private set; } = null!;

    protected QuizQuestionOption()
    {
    }

    public QuizQuestionOption(
        long questionId,
        string optionText,
        bool isCorrect,
        short sortOrder)
    {
        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(questionId));

        QuestionId = questionId;

        Update(
            optionText,
            null,
            isCorrect,
            sortOrder,
            null);
    }

    public void Update(
        string optionText,
        string? optionPinyin,
        bool isCorrect,
        short sortOrder,
        string? explanationVi)
    {
        if (string.IsNullOrWhiteSpace(optionText))
            throw new ArgumentException(
                "OptionText không được để trống.",
                nameof(optionText));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        OptionText = optionText.Trim();
        OptionPinyin = Normalize(optionPinyin);
        IsCorrect = isCorrect;
        SortOrder = sortOrder;
        ExplanationVi = Normalize(explanationVi);

        MarkUpdated();
    }

    public void MarkCorrect()
    {
        if (IsCorrect)
            return;

        IsCorrect = true;

        MarkUpdated();
    }

    public void MarkIncorrect()
    {
        if (!IsCorrect)
            return;

        IsCorrect = false;

        MarkUpdated();
    }

    public void ChangeOrder(
        short sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder = sortOrder;

        MarkUpdated();
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
