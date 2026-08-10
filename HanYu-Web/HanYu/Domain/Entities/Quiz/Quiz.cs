using HanYu.Domain.Entities;
using LessonEntity = HanYu.Domain.Entities.Lesson.Lesson;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Quiz;

public class Quiz : AuditableEntity
{
    public long? LessonId { get; private set; }

    public string TitleVi { get; private set; }
        = string.Empty;

    public string? DescriptionVi { get; private set; }

    public QuizType QuizType { get; private set; }
        = QuizType.Lesson;

    public decimal PassingScore { get; private set; }
        = 70m;

    public int? TimeLimitSeconds { get; private set; }

    public int MaxAttempts { get; private set; }

    public QuizShuffleMode ShuffleMode { get; private set; }
        = QuizShuffleMode.QuestionsAndOptions;

    public QuizFeedbackMode FeedbackMode { get; private set; }
        = QuizFeedbackMode.AfterEachAnswer;

    public bool AllowRetry { get; private set; }
        = true;

    public bool ShowCorrectAnswer { get; private set; }
        = true;

    public bool ShowExplanation { get; private set; }
        = true;

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public int Version { get; private set; }
        = 1;

    public DateTimeOffset? PublishedAt { get; private set; }

    public LessonEntity? Lesson { get; private set; }

    public ICollection<QuizQuestion> Questions { get; private set; }
        = new List<QuizQuestion>();

    protected Quiz()
    {
    }

    public Quiz(
        string titleVi,
        QuizType quizType,
        decimal passingScore = 70m)
    {
        UpdateCore(
            titleVi,
            null,
            quizType,
            passingScore,
            null,
            0);
    }

    public void UpdateCore(
        string titleVi,
        string? descriptionVi,
        QuizType quizType,
        decimal passingScore,
        int? timeLimitSeconds,
        int maxAttempts)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(titleVi))
            throw new ArgumentException(
                "TitleVi không được để trống.",
                nameof(titleVi));

        if (passingScore < 0 ||
            passingScore > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(passingScore),
                "PassingScore phải từ 0 đến 100.");
        }

        if (timeLimitSeconds.HasValue &&
            timeLimitSeconds.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeLimitSeconds));
        }

        if (maxAttempts < 0)
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts));

        TitleVi = titleVi.Trim();
        DescriptionVi = Normalize(descriptionVi);
        QuizType = quizType;
        PassingScore = passingScore;
        TimeLimitSeconds = timeLimitSeconds;
        MaxAttempts = maxAttempts;

        MarkContentChanged();
    }

    public void AttachLesson(
        long? lessonId)
    {
        EnsureEditable();

        if (lessonId.HasValue &&
            lessonId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));
        }

        LessonId = lessonId;

        MarkContentChanged();
    }

    public void ConfigureBehavior(
        QuizShuffleMode shuffleMode,
        QuizFeedbackMode feedbackMode,
        bool allowRetry,
        bool showCorrectAnswer,
        bool showExplanation)
    {
        EnsureEditable();

        ShuffleMode = shuffleMode;
        FeedbackMode = feedbackMode;
        AllowRetry = allowRetry;
        ShowCorrectAnswer = showCorrectAnswer;
        ShowExplanation = showExplanation;

        MarkContentChanged();
    }

    public void SubmitForReview()
    {
        if (Status != ContentStatus.Draft)
            throw new InvalidOperationException(
                "Chỉ Quiz Draft mới được gửi Review.");

        ValidatePublishable();

        Status = ContentStatus.Review;

        MarkUpdated();
    }

    public void Approve()
    {
        if (Status != ContentStatus.Review)
            throw new InvalidOperationException(
                "Quiz phải đang Review.");

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
                "Quiz phải được Approved trước khi Publish.");
        }

        ValidatePublishable();

        Status = ContentStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;

        Version++;

        MarkUpdated();
    }

    public void Archive()
    {
        if (Status == ContentStatus.Archived)
            return;

        if (Status != ContentStatus.Published &&
            Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Chỉ Quiz Published hoặc Approved mới có thể Archive.");
        }

        Status = ContentStatus.Archived;

        Version++;

        MarkUpdated();
    }

    public void RestoreToDraft()
    {
        if (Status != ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chỉ Quiz Archived mới có thể Restore.");
        }

        Status = ContentStatus.Draft;
        PublishedAt = null;

        Version++;

        MarkUpdated();
    }

    private void MarkContentChanged()
    {
        checked
        {
            Version++;
        }

        MarkUpdated();
    }

    private void ValidatePublishable()
    {
        if (string.IsNullOrWhiteSpace(TitleVi))
            throw new InvalidOperationException(
                "Quiz chưa có title.");

        if (Questions.Count == 0)
            throw new InvalidOperationException(
                "Quiz phải có ít nhất một câu hỏi.");
    }

    private void EnsureEditable()
    {
        if (Status == ContentStatus.Archived)
            throw new InvalidOperationException(
                "Quiz đã Archived.");
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
