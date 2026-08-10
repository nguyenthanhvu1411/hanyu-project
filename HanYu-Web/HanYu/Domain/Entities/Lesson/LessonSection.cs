using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Lesson;

public class LessonSection : AuditableEntity
{
    public long LessonId { get; private set; }

    public LessonSectionType SectionType { get; private set; }

    public string? TitleVi { get; private set; }

    public string? ContentVi { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsRequired { get; private set; }
        = true;

    public int? EstimatedSeconds { get; private set; }

    public Lesson Lesson { get; private set; } = null!;

    protected LessonSection()
    {
    }

    public LessonSection(
        long lessonId,
        LessonSectionType sectionType,
        int sortOrder,
        string? titleVi = null)
    {
        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        LessonId = lessonId;
        SectionType = sectionType;
        SortOrder = sortOrder;
        TitleVi = Normalize(titleVi);
    }

    public void UpdateContent(
        string? titleVi,
        string? contentVi)
    {
        TitleVi = Normalize(titleVi);
        ContentVi = Normalize(contentVi);

        MarkUpdated();
    }

    public void ChangeType(
        LessonSectionType sectionType)
    {
        if (SectionType == sectionType)
            return;

        SectionType = sectionType;

        MarkUpdated();
    }

    public void ChangeOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder = sortOrder;

        MarkUpdated();
    }

    public void SetRequired(bool required)
    {
        if (IsRequired == required)
            return;

        IsRequired = required;

        MarkUpdated();
    }

    public void UpdateEstimatedTime(
        int? estimatedSeconds)
    {
        if (estimatedSeconds.HasValue &&
            estimatedSeconds.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimatedSeconds));
        }

        EstimatedSeconds =
            estimatedSeconds;

        MarkUpdated();
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
