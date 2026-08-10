using Microsoft.AspNetCore.Identity;

namespace HanYu.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>
{
    public string Code { get; private set; }
        = string.Empty;

    public string DisplayName { get; private set; }
        = string.Empty;

    public string? Description { get; private set; }

    public bool IsSystem { get; private set; }

    protected Role()
    {
    }

    public Role(
        string code,
        string displayName,
        string? description = null,
        bool isSystem = false)
    {
        SetCode(
            code);

        Rename(
            displayName);

        Description =
            Normalize(
                description);

        IsSystem =
            isSystem;
    }

    public void Rename(
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(
                displayName))
        {
            throw new ArgumentException(
                "Tên vai trò không được để trống.",
                nameof(displayName));
        }

        displayName =
            displayName.Trim();

        if (displayName.Length > 100)
        {
            throw new ArgumentException(
                "Tên vai trò tối đa 100 ký tự.",
                nameof(displayName));
        }

        DisplayName =
            displayName;
    }

    public void UpdateDescription(
        string? description)
    {
        Description =
            Normalize(
                description);
    }

    private void SetCode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(
                code))
        {
            throw new ArgumentException(
                "Mã vai trò không được để trống.",
                nameof(code));
        }

        code =
            code.Trim()
                .ToUpperInvariant();

        if (code.Length > 50)
        {
            throw new ArgumentException(
                "Mã vai trò tối đa 50 ký tự.",
                nameof(code));
        }

        Code =
            code;

        // ASP.NET Identity dùng Name làm lookup role.
        Name =
            code;

        NormalizedName =
            code;
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
                value)
            ? null
            : value.Trim();
    }
}
