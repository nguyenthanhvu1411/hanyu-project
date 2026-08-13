using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Lesson;

public class LessonSectionAsset : AuditableEntity
{
    public long LessonSectionId { get; private set; }
    public long LessonAssetId { get; private set; }
    public int SortOrder { get; private set; }
    public string? CaptionVi { get; private set; }
    public bool IsRequired { get; private set; }

    public LessonSection LessonSection { get; private set; } = null!;
    public LessonAsset LessonAsset { get; private set; } = null!;

    protected LessonSectionAsset() { }

    public LessonSectionAsset(long lessonSectionId, long lessonAssetId, int sortOrder = 0, string? captionVi = null, bool isRequired = false)
    {
        if (lessonSectionId <= 0) throw new ArgumentOutOfRangeException(nameof(lessonSectionId));
        if (lessonAssetId <= 0) throw new ArgumentOutOfRangeException(nameof(lessonAssetId));
        if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));

        LessonSectionId = lessonSectionId;
        LessonAssetId = lessonAssetId;
        SortOrder = sortOrder;
        CaptionVi = Normalize(captionVi);
        IsRequired = isRequired;
    }

    public void Update(int sortOrder, string? captionVi, bool isRequired)
    {
        if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));
        SortOrder = sortOrder;
        CaptionVi = Normalize(captionVi);
        IsRequired = isRequired;
        MarkUpdated();
    }

    public void ChangeOrder(int sortOrder)
    {
        if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));
        SortOrder = sortOrder;
        MarkUpdated();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
