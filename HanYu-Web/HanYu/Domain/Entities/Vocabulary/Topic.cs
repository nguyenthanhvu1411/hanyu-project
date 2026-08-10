using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

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
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Slug không được để trống.",
                nameof(slug));
        }

        if (string.IsNullOrWhiteSpace(nameVi))
        {
            throw new ArgumentException(
                "Tên topic không được để trống.",
                nameof(nameVi));
        }

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));
        }

        Slug =
            slug.Trim()
                .ToLowerInvariant();

        NameVi =
            nameVi.Trim();

        DescriptionVi =
            string.IsNullOrWhiteSpace(
                descriptionVi)
                ? null
                : descriptionVi.Trim();

        SortOrder =
            sortOrder;

        MarkUpdated();
    }

    public void Publish()
    {
        Status =
            ContentStatus.Published;

        MarkUpdated();
    }

    public void Archive()
    {
        Status =
            ContentStatus.Archived;

        MarkUpdated();
    }

    public void Restore()
    {
        if (Status !=
            ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Topic chưa được archive.");
        }

        Status =
            ContentStatus.Draft;

        MarkUpdated();
    }
}
