namespace HanYu.Domain.Entities.Vocabulary;

public sealed class HskLevel : AuditableEntity
{
    public string Code { get; private set; }
        = string.Empty;

    public string NameVi { get; private set; }
        = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }
        = true;

    private HskLevel()
    {
    }

    public HskLevel(
        string code,
        string nameVi,
        int sortOrder)
    {
        SetCode(code);
        SetNameVi(nameVi);
        SetSortOrder(sortOrder);

        IsActive = true;
    }

    public void Update(
        string code,
        string nameVi,
        int sortOrder,
        Guid updatedById)
    {
        EnsureNotDeleted();

        SetCode(code);
        SetNameVi(nameVi);
        SetSortOrder(sortOrder);

        MarkAsUpdated(updatedById);
    }

    public void Activate(
        Guid updatedById)
    {
        EnsureNotDeleted();

        if (IsActive)
            return;

        IsActive = true;

        MarkAsUpdated(updatedById);
    }

    public void Deactivate(
        Guid updatedById)
    {
        EnsureNotDeleted();

        if (!IsActive)
            return;

        IsActive = false;

        MarkAsUpdated(updatedById);
    }

    public void Delete(
        Guid deletedById)
    {
        if (IsDeleted)
            return;

        SoftDelete(deletedById);

        IsActive = false;
    }

    public void RestoreDeleted(
        Guid restoredById)
    {
        if (!IsDeleted)
            return;

        Restore(restoredById);

        IsActive = true;
    }

    private void SetCode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "HSK code không được để trống.",
                nameof(code));
        }

        code =
            code.Trim()
                .ToUpperInvariant();

        if (code.Length > 20)
        {
            throw new ArgumentException(
                "HSK code không được vượt quá 20 ký tự.",
                nameof(code));
        }

        Code = code;
    }

    private void SetNameVi(
        string nameVi)
    {
        if (string.IsNullOrWhiteSpace(nameVi))
        {
            throw new ArgumentException(
                "Tên HSK không được để trống.",
                nameof(nameVi));
        }

        nameVi =
            nameVi.Trim();

        if (nameVi.Length > 100)
        {
            throw new ArgumentException(
                "Tên HSK không được vượt quá 100 ký tự.",
                nameof(nameVi));
        }

        NameVi = nameVi;
    }

    private void SetSortOrder(
        int sortOrder)
    {
        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "SortOrder không được âm.");
        }

        SortOrder = sortOrder;
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "HSK Level đã bị xóa.");
        }
    }
}
