using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizMatchingPair : AuditableEntity
{
    public long QuestionId { get; private set; }

    public string LeftText { get; private set; }
        = string.Empty;

    public string RightText { get; private set; }
        = string.Empty;

    public string? LeftPinyin { get; private set; }

    public string? RightPinyin { get; private set; }

    public short SortOrder { get; private set; }

    public QuizQuestion Question { get; private set; } = null!;

    protected QuizMatchingPair()
    {
    }

    public QuizMatchingPair(
        long questionId,
        string leftText,
        string rightText,
        short sortOrder)
    {
        if (questionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(questionId));

        QuestionId = questionId;

        Update(
            leftText,
            rightText,
            null,
            null,
            sortOrder);
    }

    public void Update(
        string leftText,
        string rightText,
        string? leftPinyin,
        string? rightPinyin,
        short sortOrder)
    {
        if (string.IsNullOrWhiteSpace(leftText))
            throw new ArgumentException(
                "LeftText không được để trống.",
                nameof(leftText));

        if (string.IsNullOrWhiteSpace(rightText))
            throw new ArgumentException(
                "RightText không được để trống.",
                nameof(rightText));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        LeftText = leftText.Trim();
        RightText = rightText.Trim();

        LeftPinyin = Normalize(leftPinyin);
        RightPinyin = Normalize(rightPinyin);

        SortOrder = sortOrder;

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
