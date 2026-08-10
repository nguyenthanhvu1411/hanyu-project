using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Course;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Lesson;

public class Lesson : AuditableEntity
{
    public long HskLevelId { get; private set; }

    public long? TopicId { get; private set; }

    public string Slug { get; private set; }
        = string.Empty;

    public string TitleVi { get; private set; }
        = string.Empty;

    public string? ShortDescriptionVi { get; private set; }

    public string? DescriptionVi { get; private set; }

    public string? ObjectiveVi { get; private set; }

    public string? CoverImageUrl { get; private set; }

    public int SortOrder { get; private set; }

    public short EstimatedMinutes { get; private set; }
        = 15;

    public short Difficulty { get; private set; }
        = 1;

    public bool IsFeatured { get; private set; }

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public int Version { get; private set; } = 1;

    public DateTimeOffset? PublishedAt { get; private set; }

    public HskLevel HskLevel { get; private set; } = null!;

    public Topic? Topic { get; private set; }

    public long? CourseChapterId { get; private set; }

    public CourseChapter? CourseChapter { get; private set; }

    public ICollection<LessonSection> Sections { get; private set; }
        = new List<LessonSection>();

    public ICollection<LessonVocabulary> LessonVocabularies { get; private set; }
        = new List<LessonVocabulary>();

    public ICollection<LessonAsset> Assets { get; private set; }
        = new List<LessonAsset>();

    public ICollection<LessonPrerequisite> Prerequisites { get; private set; }
        = new List<LessonPrerequisite>();

    protected Lesson()
    {
    }

    public Lesson(
        long hskLevelId,
        string slug,
        string titleVi,
        int sortOrder = 0)
    {
        UpdateCore(
            hskLevelId,
            slug,
            titleVi,
            null,
            null,
            null,
            sortOrder,
            15,
            1);
    }

    public void UpdateCore(
        long hskLevelId,
        string slug,
        string titleVi,
        string? shortDescriptionVi,
        string? descriptionVi,
        string? objectiveVi,
        int sortOrder,
        short estimatedMinutes,
        short difficulty)
    {
        EnsureEditable();

        if (hskLevelId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hskLevelId),
                "HskLevelId phải lớn hơn 0.");
        }

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException(
                "Slug không được để trống.",
                nameof(slug));

        if (string.IsNullOrWhiteSpace(titleVi))
            throw new ArgumentException(
                "TitleVi không được để trống.",
                nameof(titleVi));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        if (estimatedMinutes < 1 ||
            estimatedMinutes > 300)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimatedMinutes));
        }

        if (difficulty < 1 ||
            difficulty > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(difficulty),
                "Difficulty phải từ 1 đến 5.");
        }

        HskLevelId = hskLevelId;
        Slug = NormalizeSlug(slug);
        TitleVi = titleVi.Trim();

        ShortDescriptionVi =
            Normalize(shortDescriptionVi);

        DescriptionVi =
            Normalize(descriptionVi);

        ObjectiveVi =
            Normalize(objectiveVi);

        SortOrder = sortOrder;
        EstimatedMinutes = estimatedMinutes;
        Difficulty = difficulty;

        MarkContentChanged();
    }

    public void AssignToChapter(
        long courseChapterId,
        int sortOrder)
    {
        EnsureEditable();

        if (courseChapterId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(courseChapterId),
                "CourseChapterId không hợp lệ.");
        }

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "SortOrder không được âm.");
        }

        if (
            CourseChapterId == courseChapterId &&
            SortOrder == sortOrder)
        {
            return;
        }

        CourseChapterId =
            courseChapterId;

        SortOrder =
            sortOrder;

        MarkContentChanged();
    }

    public void AssignCourseChapter(
        long? courseChapterId)
    {
        EnsureEditable();

        if (
            courseChapterId.HasValue &&
            courseChapterId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(courseChapterId),
                "CourseChapterId không hợp lệ.");
        }

        if (CourseChapterId == courseChapterId)
        {
            return;
        }

        CourseChapterId =
            courseChapterId;

        MarkContentChanged();
    }

    public void MoveToChapter(
        long courseChapterId,
        int sortOrder)
    {
        AssignToChapter(
            courseChapterId,
            sortOrder);
    }

    public void RemoveFromChapter()
    {
        EnsureEditable();

        if (!CourseChapterId.HasValue)
        {
            return;
        }

        CourseChapterId =
            null;

        SortOrder =
            0;

        MarkContentChanged();
    }

    public void AssignTopic(long? topicId)
    {
        EnsureEditable();

        if (topicId.HasValue &&
            topicId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topicId));
        }

        TopicId = topicId;

        MarkContentChanged();
    }

    public void UpdateCover(
        string? coverImageUrl)
    {
        EnsureEditable();

        coverImageUrl =
            Normalize(coverImageUrl);

        if (coverImageUrl?.Length > 2048)
            throw new ArgumentException(
                "CoverImageUrl quá dài.",
                nameof(coverImageUrl));

        CoverImageUrl =
            coverImageUrl;

        MarkContentChanged();
    }

    public void SetFeatured(bool featured)
    {
        if (IsFeatured == featured)
            return;

        IsFeatured = featured;

        MarkContentChanged();
    }

    public void ChangeOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder = sortOrder;

        MarkContentChanged();
    }

    public void SubmitForReview()
    {
        if (Status != ContentStatus.Draft)
            throw new InvalidOperationException(
                "Chỉ lesson Draft mới có thể gửi Review.");

        ValidatePublishable();

        Status = ContentStatus.Review;

        MarkUpdated();
    }

    public void Approve()
    {
        if (Status != ContentStatus.Review)
            throw new InvalidOperationException(
                "Chỉ lesson đang Review mới có thể Approve.");

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
                "Lesson phải được Approved trước khi Publish.");
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
                "Chỉ Lesson Published hoặc Approved mới có thể Archive.");
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
                "Chỉ Lesson Archived mới có thể Restore.");
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
        if (string.IsNullOrWhiteSpace(Slug))
            throw new InvalidOperationException(
                "Lesson chưa có Slug.");

        if (string.IsNullOrWhiteSpace(TitleVi))
            throw new InvalidOperationException(
                "Lesson chưa có tiêu đề.");

        if (HskLevelId <= 0)
        {
            throw new InvalidOperationException(
                "HSK level không hợp lệ.");
        }
    }

    private void EnsureEditable()
    {
        if (Status == ContentStatus.Archived)
            throw new InvalidOperationException(
                "Lesson đã Archived.");
    }

    private static string NormalizeSlug(string value)
        => string.Join(
            '-',
            value.Trim()
                .ToLowerInvariant()
                .Split(
                    [' ', '-'],
                    StringSplitOptions.RemoveEmptyEntries));

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
