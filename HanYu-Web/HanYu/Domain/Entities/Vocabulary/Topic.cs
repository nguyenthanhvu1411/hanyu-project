using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

/// <summary>
/// Shared content taxonomy used across Lesson, Vocabulary and other learning modules.
/// The CLR namespace is kept for migration compatibility; business usage is no longer
/// limited to the Vocabulary module.
/// </summary>
public class Topic : AuditableEntity
{
    public string Slug { get; private set; }
        = string.Empty;

    public string NameVi { get; private set; }
        = string.Empty;

    public string? DescriptionVi { get; private set; }

    public int SortOrder { get; private set; }

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    protected Topic()
    {
    }

    public Topic(
        string slug,
        string nameVi,
        string? descriptionVi = null,
        int sortOrder = 0)
    {
        Update(
            slug,
            nameVi,
            descriptionVi,
            sortOrder);
    }

    public void Update(
        string slug,
        string nameVi,
        string? descriptionVi,
        int sortOrder)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Slug không được để trống.",
                nameof(slug));
        }

        if (string.IsNullOrWhiteSpace(nameVi))
        {
            throw new ArgumentException(
                "Tên chủ đề không được để trống.",
                nameof(nameVi));
        }

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));
        }

        var normalizedSlug = NormalizeSlug(slug);
        var normalizedName = nameVi.Trim();
        var normalizedDescription = Normalize(descriptionVi);

        if (Slug == normalizedSlug &&
            NameVi == normalizedName &&
            DescriptionVi == normalizedDescription &&
            SortOrder == sortOrder)
        {
            return;
        }

        Slug = normalizedSlug;
        NameVi = normalizedName;
        DescriptionVi = normalizedDescription;
        SortOrder = sortOrder;

        MarkUpdated();
    }

    public void Publish()
    {
        if (Status == ContentStatus.Published)
        {
            return;
        }

        if (Status != ContentStatus.Draft &&
            Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Chỉ chủ đề Draft hoặc Approved mới có thể Publish.");
        }

        Status = ContentStatus.Published;
        MarkUpdated();
    }

    public void Archive()
    {
        if (Status == ContentStatus.Archived)
        {
            return;
        }

        if (Status != ContentStatus.Published)
        {
            throw new InvalidOperationException(
                "Chỉ chủ đề Published mới có thể Archive.");
        }

        Status = ContentStatus.Archived;
        MarkUpdated();
    }

    public void Restore()
    {
        if (Status != ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chủ đề chưa được Archive.");
        }

        Status = ContentStatus.Draft;
        MarkUpdated();
    }

    private void EnsureEditable()
    {
        if (Status == ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chủ đề Archived không thể chỉnh sửa. Hãy khôi phục về Draft trước.");
        }
    }

    private static string NormalizeSlug(string value)
        => string.Join(
            '-',
            value.Trim()
                .ToLowerInvariant()
                .Split(
                    [' ', '-', '_'],
                    StringSplitOptions.RemoveEmptyEntries));

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
