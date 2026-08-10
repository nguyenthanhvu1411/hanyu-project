using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Gamification;

public class Achievement : AuditableEntity
{
    public string Code { get; private set; }
        = string.Empty;

    public string NameVi { get; private set; }
        = string.Empty;

    public string? DescriptionVi { get; private set; }

    public string? IconUrl { get; private set; }

    public int XpReward { get; private set; }

    public bool IsActive { get; private set; }
        = true;

    public int SortOrder { get; private set; }

    protected Achievement()
    {
    }

    public Achievement(
        string code,
        string nameVi,
        int xpReward = 0,
        int sortOrder = 0)
    {
        Update(
            code,
            nameVi,
            null,
            null,
            xpReward,
            sortOrder);
    }

    public void Update(
        string code,
        string nameVi,
        string? descriptionVi,
        string? iconUrl,
        int xpReward,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Code không được để trống.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(nameVi))
            throw new ArgumentException(
                "Tên achievement không được để trống.",
                nameof(nameVi));

        if (xpReward < 0)
            throw new ArgumentOutOfRangeException(
                nameof(xpReward));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        Code = code.Trim().ToUpperInvariant();
        NameVi = nameVi.Trim();

        DescriptionVi = Normalize(descriptionVi);
        IconUrl = Normalize(iconUrl);

        XpReward = xpReward;
        SortOrder = sortOrder;

        MarkUpdated();
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        MarkUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        MarkUpdated();
    }

    public void ChangeReward(int xpReward)
    {
        if (xpReward < 0)
            throw new ArgumentOutOfRangeException(
                nameof(xpReward));

        XpReward = xpReward;

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

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}