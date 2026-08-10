namespace HanYu.Domain.Entities.Identity;

public class Permission
{
    public long Id { get; private set; }

    public string Code { get; private set; }
        = string.Empty;

    public string Resource { get; private set; }
        = string.Empty;

    public string Action { get; private set; }
        = string.Empty;

    public string? Description { get; private set; }

    protected Permission()
    {
    }

    public Permission(
        string code,
        string resource,
        string action,
        string? description = null)
    {
        SetCode(
            code);

        Update(
            resource,
            action,
            description);
    }

    public void Update(
        string resource,
        string action,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(
                resource))
        {
            throw new ArgumentException(
                "Resource không được để trống.",
                nameof(resource));
        }

        if (string.IsNullOrWhiteSpace(
                action))
        {
            throw new ArgumentException(
                "Action không được để trống.",
                nameof(action));
        }

        Resource =
            resource.Trim();

        Action =
            action.Trim();

        Description =
            string.IsNullOrWhiteSpace(
                description)
                ? null
                : description.Trim();
    }

    private void SetCode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(
                code))
        {
            throw new ArgumentException(
                "Permission code không được để trống.",
                nameof(code));
        }

        code =
            code.Trim()
                .ToLowerInvariant();

        if (code.Length > 100)
        {
            throw new ArgumentException(
                "Permission code không được vượt quá 100 ký tự.",
                nameof(code));
        }

        Code =
            code;
    }
}
