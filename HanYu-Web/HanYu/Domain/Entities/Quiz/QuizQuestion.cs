using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Quiz;

public class QuizQuestion : AuditableEntity
{
    public long QuizId { get; private set; }

    public long? VocabularyId { get; private set; }

    public QuizQuestionType QuestionType { get; private set; }

    public string Prompt { get; private set; }
        = string.Empty;

    public string? PromptPinyin { get; private set; }

    public string? CorrectAnswerText { get; private set; }

    public string? ExplanationVi { get; private set; }

    public string? HintVi { get; private set; }

    public decimal Points { get; private set; }
        = 1m;

    public int SortOrder { get; private set; }

    public int? TimeLimitSeconds { get; private set; }

    public bool IsRequired { get; private set; }
        = true;

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public Quiz Quiz { get; private set; } = null!;

    public HanYu.Domain.Entities.Vocabulary.Vocabulary? Vocabulary { get; private set; }

    public ICollection<QuizQuestionOption> Options { get; private set; }
        = new List<QuizQuestionOption>();

    public ICollection<QuizMatchingPair> MatchingPairs { get; private set; }
        = new List<QuizMatchingPair>();

    protected QuizQuestion()
    {
    }

    public QuizQuestion(
        long quizId,
        QuizQuestionType questionType,
        string prompt,
        decimal points,
        int sortOrder)
    {
        if (quizId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quizId));

        QuizId = quizId;

        QuestionType = questionType;

        UpdateConfiguration(
            prompt,
            null,
            null,
            null,
            null,
            points,
            sortOrder,
            null,
            true);
    }

    public void UpdateConfiguration(
        string prompt,
        string? promptPinyin,
        string? correctAnswerText,
        string? explanationVi,
        string? hintVi,
        decimal points,
        int sortOrder,
        int? timeLimitSeconds,
        bool isRequired)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException(
                "Prompt không được để trống.",
                nameof(prompt));

        if (points <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(points));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        if (timeLimitSeconds.HasValue &&
            timeLimitSeconds.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeLimitSeconds));
        }

        Prompt = prompt.Trim();
        PromptPinyin = Normalize(promptPinyin);
        CorrectAnswerText = Normalize(correctAnswerText);
        ExplanationVi = Normalize(explanationVi);
        HintVi = Normalize(hintVi);

        Points = points;
        SortOrder = sortOrder;
        TimeLimitSeconds = timeLimitSeconds;
        IsRequired = isRequired;

        MarkUpdated();
    }

    public void ChangeQuestionType(
        QuizQuestionType questionType)
    {
        EnsureEditable();

        QuestionType = questionType;

        MarkUpdated();
    }

    public void AttachVocabulary(
        long? vocabularyId)
    {
        EnsureEditable();

        if (vocabularyId.HasValue &&
            vocabularyId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));
        }

        VocabularyId = vocabularyId;

        MarkUpdated();
    }

    public void SubmitForReview()
    {
        if (Status != ContentStatus.Draft)
        {
            throw new InvalidOperationException(
                "Chỉ Question Draft mới có thể gửi Review.");
        }

        ValidatePublishable();

        Status = ContentStatus.Review;

        MarkUpdated();
    }

    public void Approve()
    {
        if (Status != ContentStatus.Review)
        {
            throw new InvalidOperationException(
                "Question phải đang Review.");
        }

        Status = ContentStatus.Approved;

        MarkUpdated();
    }

    public void Publish()
    {
        if (Status == ContentStatus.Published)
            return;

        if (Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Question phải được Approved trước khi Publish.");
        }

        ValidatePublishable();

        Status = ContentStatus.Published;

        MarkUpdated();
    }

    public void Archive()
    {
        if (Status == ContentStatus.Archived)
            return;

        Status = ContentStatus.Archived;

        MarkUpdated();
    }

    public void RestoreToDraft()
    {
        if (Status != ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Question chưa Archived.");
        }

        Status = ContentStatus.Draft;

        MarkUpdated();
    }

    private void ValidatePublishable()
    {
        if (string.IsNullOrWhiteSpace(Prompt))
            throw new InvalidOperationException(
                "Question chưa có Prompt.");

        if (Points <= 0)
            throw new InvalidOperationException(
                "Points không hợp lệ.");
    }

    private void EnsureEditable()
    {
        if (Status == ContentStatus.Archived)
            throw new InvalidOperationException(
                "Question đã Archived.");
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
